/* */ 
"format cjs";
import { isPresent, isBlank, looseIdentical } from 'angular2/src/facade/lang';
import { ListWrapper, Map } from 'angular2/src/facade/collection';
import { RecordType, ProtoRecord } from './proto_record';
/**
 * Removes "duplicate" records. It assumes that record evaluation does not have side-effects.
 *
 * Records that are not last in bindings are removed and all the indices of the records that depend
 * on them are updated.
 *
 * Records that are last in bindings CANNOT be removed, and instead are replaced with very cheap
 * SELF records.
 *
 * @internal
 */
export function coalesce(srcRecords) {
    let dstRecords = [];
    let excludedIdxs = [];
    let indexMap = new Map();
    let skipDepth = 0;
    let skipSources = ListWrapper.createFixedSize(srcRecords.length);
    for (let protoIndex = 0; protoIndex < srcRecords.length; protoIndex++) {
        let skipRecord = skipSources[protoIndex];
        if (isPresent(skipRecord)) {
            skipDepth--;
            skipRecord.fixedArgs[0] = dstRecords.length;
        }
        let src = srcRecords[protoIndex];
        let dst = _cloneAndUpdateIndexes(src, dstRecords, indexMap);
        if (dst.isSkipRecord()) {
            dstRecords.push(dst);
            skipDepth++;
            skipSources[dst.fixedArgs[0]] = dst;
        }
        else {
            let record = _mayBeAddRecord(dst, dstRecords, excludedIdxs, skipDepth > 0);
            indexMap.set(src.selfIndex, record.selfIndex);
        }
    }
    return _optimizeSkips(dstRecords);
}
/**
 * - Conditional skip of 1 record followed by an unconditional skip of N are replaced by  a
 *   conditional skip of N with the negated condition,
 * - Skips of 0 records are removed
 */
function _optimizeSkips(srcRecords) {
    let dstRecords = [];
    let skipSources = ListWrapper.createFixedSize(srcRecords.length);
    let indexMap = new Map();
    for (let protoIndex = 0; protoIndex < srcRecords.length; protoIndex++) {
        let skipRecord = skipSources[protoIndex];
        if (isPresent(skipRecord)) {
            skipRecord.fixedArgs[0] = dstRecords.length;
        }
        let src = srcRecords[protoIndex];
        if (src.isSkipRecord()) {
            if (src.isConditionalSkipRecord() && src.fixedArgs[0] === protoIndex + 2 &&
                protoIndex < srcRecords.length - 1 &&
                srcRecords[protoIndex + 1].mode === RecordType.SkipRecords) {
                src.mode = src.mode === RecordType.SkipRecordsIf ? RecordType.SkipRecordsIfNot :
                    RecordType.SkipRecordsIf;
                src.fixedArgs[0] = srcRecords[protoIndex + 1].fixedArgs[0];
                protoIndex++;
            }
            if (src.fixedArgs[0] > protoIndex + 1) {
                let dst = _cloneAndUpdateIndexes(src, dstRecords, indexMap);
                dstRecords.push(dst);
                skipSources[dst.fixedArgs[0]] = dst;
            }
        }
        else {
            let dst = _cloneAndUpdateIndexes(src, dstRecords, indexMap);
            dstRecords.push(dst);
            indexMap.set(src.selfIndex, dst.selfIndex);
        }
    }
    return dstRecords;
}
/**
 * Add a new record or re-use one of the existing records.
 */
function _mayBeAddRecord(record, dstRecords, excludedIdxs, excluded) {
    let match = _findFirstMatch(record, dstRecords, excludedIdxs);
    if (isPresent(match)) {
        if (record.lastInBinding) {
            dstRecords.push(_createSelfRecord(record, match.selfIndex, dstRecords.length + 1));
            match.referencedBySelf = true;
        }
        else {
            if (record.argumentToPureFunction) {
                match.argumentToPureFunction = true;
            }
        }
        return match;
    }
    if (excluded) {
        excludedIdxs.push(record.selfIndex);
    }
    dstRecords.push(record);
    return record;
}
/**
 * Returns the first `ProtoRecord` that matches the record.
 */
function _findFirstMatch(record, dstRecords, excludedIdxs) {
    return dstRecords.find(
    // TODO(vicb): optimize excludedIdxs.indexOf (sorted array)
    rr => excludedIdxs.indexOf(rr.selfIndex) == -1 && rr.mode !== RecordType.DirectiveLifecycle &&
        _haveSameDirIndex(rr, record) && rr.mode === record.mode &&
        looseIdentical(rr.funcOrValue, record.funcOrValue) &&
        rr.contextIndex === record.contextIndex && looseIdentical(rr.name, record.name) &&
        ListWrapper.equals(rr.args, record.args));
}
/**
 * Clone the `ProtoRecord` and changes the indexes for the ones in the destination array for:
 * - the arguments,
 * - the context,
 * - self
 */
function _cloneAndUpdateIndexes(record, dstRecords, indexMap) {
    let args = record.args.map(src => _srcToDstSelfIndex(indexMap, src));
    let contextIndex = _srcToDstSelfIndex(indexMap, record.contextIndex);
    let selfIndex = dstRecords.length + 1;
    return new ProtoRecord(record.mode, record.name, record.funcOrValue, args, record.fixedArgs, contextIndex, record.directiveIndex, selfIndex, record.bindingRecord, record.lastInBinding, record.lastInDirective, record.argumentToPureFunction, record.referencedBySelf, record.propertyBindingIndex);
}
/**
 * Returns the index in the destination array corresponding to the index in the src array.
 * When the element is not present in the destination array, return the source index.
 */
function _srcToDstSelfIndex(indexMap, srcIdx) {
    var dstIdx = indexMap.get(srcIdx);
    return isPresent(dstIdx) ? dstIdx : srcIdx;
}
function _createSelfRecord(r, contextIndex, selfIndex) {
    return new ProtoRecord(RecordType.Self, "self", null, [], r.fixedArgs, contextIndex, r.directiveIndex, selfIndex, r.bindingRecord, r.lastInBinding, r.lastInDirective, false, false, r.propertyBindingIndex);
}
function _haveSameDirIndex(a, b) {
    var di1 = isBlank(a.directiveIndex) ? null : a.directiveIndex.directiveIndex;
    var ei1 = isBlank(a.directiveIndex) ? null : a.directiveIndex.elementIndex;
    var di2 = isBlank(b.directiveIndex) ? null : b.directiveIndex.directiveIndex;
    var ei2 = isBlank(b.directiveIndex) ? null : b.directiveIndex.elementIndex;
    return di1 === di2 && ei1 === ei2;
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY29hbGVzY2UuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJhbmd1bGFyMi9zcmMvY29yZS9jaGFuZ2VfZGV0ZWN0aW9uL2NvYWxlc2NlLnRzIl0sIm5hbWVzIjpbImNvYWxlc2NlIiwiX29wdGltaXplU2tpcHMiLCJfbWF5QmVBZGRSZWNvcmQiLCJfZmluZEZpcnN0TWF0Y2giLCJfY2xvbmVBbmRVcGRhdGVJbmRleGVzIiwiX3NyY1RvRHN0U2VsZkluZGV4IiwiX2NyZWF0ZVNlbGZSZWNvcmQiLCJfaGF2ZVNhbWVEaXJJbmRleCJdLCJtYXBwaW5ncyI6Ik9BQU8sRUFBQyxTQUFTLEVBQUUsT0FBTyxFQUFFLGNBQWMsRUFBQyxNQUFNLDBCQUEwQjtPQUNwRSxFQUFDLFdBQVcsRUFBRSxHQUFHLEVBQUMsTUFBTSxnQ0FBZ0M7T0FDeEQsRUFBQyxVQUFVLEVBQUUsV0FBVyxFQUFDLE1BQU0sZ0JBQWdCO0FBRXREOzs7Ozs7Ozs7O0dBVUc7QUFDSCx5QkFBeUIsVUFBeUI7SUFDaERBLElBQUlBLFVBQVVBLEdBQUdBLEVBQUVBLENBQUNBO0lBQ3BCQSxJQUFJQSxZQUFZQSxHQUFHQSxFQUFFQSxDQUFDQTtJQUN0QkEsSUFBSUEsUUFBUUEsR0FBd0JBLElBQUlBLEdBQUdBLEVBQWtCQSxDQUFDQTtJQUM5REEsSUFBSUEsU0FBU0EsR0FBR0EsQ0FBQ0EsQ0FBQ0E7SUFDbEJBLElBQUlBLFdBQVdBLEdBQWtCQSxXQUFXQSxDQUFDQSxlQUFlQSxDQUFDQSxVQUFVQSxDQUFDQSxNQUFNQSxDQUFDQSxDQUFDQTtJQUVoRkEsR0FBR0EsQ0FBQ0EsQ0FBQ0EsR0FBR0EsQ0FBQ0EsVUFBVUEsR0FBR0EsQ0FBQ0EsRUFBRUEsVUFBVUEsR0FBR0EsVUFBVUEsQ0FBQ0EsTUFBTUEsRUFBRUEsVUFBVUEsRUFBRUEsRUFBRUEsQ0FBQ0E7UUFDdEVBLElBQUlBLFVBQVVBLEdBQUdBLFdBQVdBLENBQUNBLFVBQVVBLENBQUNBLENBQUNBO1FBQ3pDQSxFQUFFQSxDQUFDQSxDQUFDQSxTQUFTQSxDQUFDQSxVQUFVQSxDQUFDQSxDQUFDQSxDQUFDQSxDQUFDQTtZQUMxQkEsU0FBU0EsRUFBRUEsQ0FBQ0E7WUFDWkEsVUFBVUEsQ0FBQ0EsU0FBU0EsQ0FBQ0EsQ0FBQ0EsQ0FBQ0EsR0FBR0EsVUFBVUEsQ0FBQ0EsTUFBTUEsQ0FBQ0E7UUFDOUNBLENBQUNBO1FBRURBLElBQUlBLEdBQUdBLEdBQUdBLFVBQVVBLENBQUNBLFVBQVVBLENBQUNBLENBQUNBO1FBQ2pDQSxJQUFJQSxHQUFHQSxHQUFHQSxzQkFBc0JBLENBQUNBLEdBQUdBLEVBQUVBLFVBQVVBLEVBQUVBLFFBQVFBLENBQUNBLENBQUNBO1FBRTVEQSxFQUFFQSxDQUFDQSxDQUFDQSxHQUFHQSxDQUFDQSxZQUFZQSxFQUFFQSxDQUFDQSxDQUFDQSxDQUFDQTtZQUN2QkEsVUFBVUEsQ0FBQ0EsSUFBSUEsQ0FBQ0EsR0FBR0EsQ0FBQ0EsQ0FBQ0E7WUFDckJBLFNBQVNBLEVBQUVBLENBQUNBO1lBQ1pBLFdBQVdBLENBQUNBLEdBQUdBLENBQUNBLFNBQVNBLENBQUNBLENBQUNBLENBQUNBLENBQUNBLEdBQUdBLEdBQUdBLENBQUNBO1FBQ3RDQSxDQUFDQTtRQUFDQSxJQUFJQSxDQUFDQSxDQUFDQTtZQUNOQSxJQUFJQSxNQUFNQSxHQUFHQSxlQUFlQSxDQUFDQSxHQUFHQSxFQUFFQSxVQUFVQSxFQUFFQSxZQUFZQSxFQUFFQSxTQUFTQSxHQUFHQSxDQUFDQSxDQUFDQSxDQUFDQTtZQUMzRUEsUUFBUUEsQ0FBQ0EsR0FBR0EsQ0FBQ0EsR0FBR0EsQ0FBQ0EsU0FBU0EsRUFBRUEsTUFBTUEsQ0FBQ0EsU0FBU0EsQ0FBQ0EsQ0FBQ0E7UUFDaERBLENBQUNBO0lBQ0hBLENBQUNBO0lBRURBLE1BQU1BLENBQUNBLGNBQWNBLENBQUNBLFVBQVVBLENBQUNBLENBQUNBO0FBQ3BDQSxDQUFDQTtBQUVEOzs7O0dBSUc7QUFDSCx3QkFBd0IsVUFBeUI7SUFDL0NDLElBQUlBLFVBQVVBLEdBQUdBLEVBQUVBLENBQUNBO0lBQ3BCQSxJQUFJQSxXQUFXQSxHQUFHQSxXQUFXQSxDQUFDQSxlQUFlQSxDQUFDQSxVQUFVQSxDQUFDQSxNQUFNQSxDQUFDQSxDQUFDQTtJQUNqRUEsSUFBSUEsUUFBUUEsR0FBd0JBLElBQUlBLEdBQUdBLEVBQWtCQSxDQUFDQTtJQUU5REEsR0FBR0EsQ0FBQ0EsQ0FBQ0EsR0FBR0EsQ0FBQ0EsVUFBVUEsR0FBR0EsQ0FBQ0EsRUFBRUEsVUFBVUEsR0FBR0EsVUFBVUEsQ0FBQ0EsTUFBTUEsRUFBRUEsVUFBVUEsRUFBRUEsRUFBRUEsQ0FBQ0E7UUFDdEVBLElBQUlBLFVBQVVBLEdBQUdBLFdBQVdBLENBQUNBLFVBQVVBLENBQUNBLENBQUNBO1FBQ3pDQSxFQUFFQSxDQUFDQSxDQUFDQSxTQUFTQSxDQUFDQSxVQUFVQSxDQUFDQSxDQUFDQSxDQUFDQSxDQUFDQTtZQUMxQkEsVUFBVUEsQ0FBQ0EsU0FBU0EsQ0FBQ0EsQ0FBQ0EsQ0FBQ0EsR0FBR0EsVUFBVUEsQ0FBQ0EsTUFBTUEsQ0FBQ0E7UUFDOUNBLENBQUNBO1FBRURBLElBQUlBLEdBQUdBLEdBQUdBLFVBQVVBLENBQUNBLFVBQVVBLENBQUNBLENBQUNBO1FBRWpDQSxFQUFFQSxDQUFDQSxDQUFDQSxHQUFHQSxDQUFDQSxZQUFZQSxFQUFFQSxDQUFDQSxDQUFDQSxDQUFDQTtZQUN2QkEsRUFBRUEsQ0FBQ0EsQ0FBQ0EsR0FBR0EsQ0FBQ0EsdUJBQXVCQSxFQUFFQSxJQUFJQSxHQUFHQSxDQUFDQSxTQUFTQSxDQUFDQSxDQUFDQSxDQUFDQSxLQUFLQSxVQUFVQSxHQUFHQSxDQUFDQTtnQkFDcEVBLFVBQVVBLEdBQUdBLFVBQVVBLENBQUNBLE1BQU1BLEdBQUdBLENBQUNBO2dCQUNsQ0EsVUFBVUEsQ0FBQ0EsVUFBVUEsR0FBR0EsQ0FBQ0EsQ0FBQ0EsQ0FBQ0EsSUFBSUEsS0FBS0EsVUFBVUEsQ0FBQ0EsV0FBV0EsQ0FBQ0EsQ0FBQ0EsQ0FBQ0E7Z0JBQy9EQSxHQUFHQSxDQUFDQSxJQUFJQSxHQUFHQSxHQUFHQSxDQUFDQSxJQUFJQSxLQUFLQSxVQUFVQSxDQUFDQSxh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YyxFQUFFLENBQWM7SUFDdkRDLElBQUlBLEdBQUdBLEdBQUdBLE9BQU9BLENBQUNBLENBQUNBLENBQUNBLGNBQWNBLENBQUNBLEdBQUdBLElBQUlBLEdBQUdBLENBQUNBLENBQUNBLGNBQWNBLENBQUNBLGNBQWNBLENBQUNBO0lBQzdFQSxJQUFJQSxHQUFHQSxHQUFHQSxPQUFPQSxDQUFDQSxDQUFDQSxDQUFDQSxjQUFjQSxDQUFDQSxHQUFHQSxJQUFJQSxHQUFHQSxDQUFDQSxDQUFDQSxjQUFjQSxDQUFDQSxZQUFZQSxDQUFDQTtJQUUzRUEsSUFBSUEsR0FBR0EsR0FBR0EsT0FBT0EsQ0FBQ0EsQ0FBQ0EsQ0FBQ0EsY0FBY0EsQ0FBQ0EsR0FBR0EsSUFBSUEsR0FBR0EsQ0FBQ0EsQ0FBQ0EsY0FBY0EsQ0FBQ0EsY0FBY0EsQ0FBQ0E7SUFDN0VBLElBQUlBLEdBQUdBLEdBQUdBLE9BQU9BLENBQUNBLENBQUNBLENBQUNBLGNBQWNBLENBQUNBLEdBQUdBLElBQUlBLEdBQUdBLENBQUNBLENBQUNBLGNBQWNBLENBQUNBLFlBQVlBLENBQUNBO0lBRTNFQSxNQUFNQSxDQUFDQSxHQUFHQSxLQUFLQSxHQUFHQSxJQUFJQSxHQUFHQSxLQUFLQSxHQUFHQSxDQUFDQTtBQUNwQ0EsQ0FBQ0EiLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQge2lzUHJlc2VudCwgaXNCbGFuaywgbG9vc2VJZGVudGljYWx9IGZyb20gJ2FuZ3VsYXIyL3NyYy9mYWNhZGUvbGFuZyc7XG5pbXBvcnQge0xpc3RXcmFwcGVyLCBNYXB9IGZyb20gJ2FuZ3VsYXIyL3NyYy9mYWNhZGUvY29sbGVjdGlvbic7XG5pbXBvcnQge1JlY29yZFR5cGUsIFByb3RvUmVjb3JkfSBmcm9tICcuL3Byb3RvX3JlY29yZCc7XG5cbi8qKlxuICogUmVtb3ZlcyBcImR1cGxpY2F0ZVwiIHJlY29yZHMuIEl0IGFzc3VtZXMgdGhhdCByZWNvcmQgZXZhbHVhdGlvbiBkb2VzIG5vdCBoYXZlIHNpZGUtZWZmZWN0cy5cbiAqXG4gKiBSZWNvcmRzIHRoYXQgYXJlIG5vdCBsYXN0IGluIGJpbmRpbmdzIGFyZSByZW1vdmVkIGFuZCBhbGwgdGhlIGluZGljZXMgb2YgdGhlIHJlY29yZHMgdGhhdCBkZXBlbmRcbiAqIG9uIHRoZW0gYXJlIHVwZGF0ZWQuXG4gKlxuICogUmVjb3JkcyB0aGF0IGFyZSBsYXN0IGluIGJpbmRpbmdzIENBTk5PVCBiZSByZW1vdmVkLCBhbmQgaW5zdGVhZCBhcmUgcmVwbGFjZWQgd2l0aCB2ZXJ5IGNoZWFwXG4gKiBTRUxGIHJlY29yZHMuXG4gKlxuICogQGludGVybmFsXG4gKi9cbmV4cG9ydCBmdW5jdGlvbiBjb2FsZXNjZShzcmNSZWNvcmRzOiBQcm90b1JlY29yZFtdKTogUHJvdG9SZWNvcmRbXSB7XG4gIGxldCBkc3RSZWNvcmRzID0gW107XG4gIGxldCBleGNsdWRlZElkeHMgPSBbXTtcbiAgbGV0IGluZGV4TWFwOiBNYXA8bnVtYmVyLCBudW1iZXI+ID0gbmV3IE1hcDxudW1iZXIsIG51bWJlcj4oKTtcbiAgbGV0IHNraXBEZXB0aCA9IDA7XG4gIGxldCBza2lwU291cmNlczogUHJvdG9SZWNvcmRbXSA9IExpc3RXcmFwcGVyLmNyZWF0ZUZpeGVkU2l6ZShzcmNSZWNvcmRzLmxlbmd0aCk7XG5cbiAgZm9yIChsZXQgcHJvdG9JbmRleCA9IDA7IHByb3RvSW5kZXggPCBzcmNSZWNvcmRzLmxlbmd0aDsgcHJvdG9JbmRleCsrKSB7XG4gICAgbGV0IHNraXBSZWNvcmQgPSBza2lwU291cmNlc1twcm90b0luZGV4XTtcbiAgICBpZiAoaXNQcmVzZW50KHNraXBSZWNvcmQpKSB7XG4gICAgICBza2lwRGVwdGgtLTtcbiAgICAgIHNraXBSZWNvcmQuZml4ZWRBcmdzWzBdID0gZHN0UmVjb3Jkcy5sZW5ndGg7XG4gICAgfVxuXG4gICAgbGV0IHNyYyA9IHNyY1JlY29yZHNbcHJvdG9JbmRleF07XG4gICAgbGV0IGRzdCA9IF9jbG9uZUFuZFVwZGF0ZUluZGV4ZXMoc3JjLCBkc3RSZWNvcmRzLCBpbmRleE1hcCk7XG5cbiAgICBpZiAoZHN0LmlzU2tpcFJlY29yZCgpKSB7XG4gICAgICBkc3RSZWNvcmRzLnB1c2goZHN0KTtcbiAgICAgIHNraXBEZXB0aCsrO1xuICAgICAgc2tpcFNvdXJjZXNbZHN0LmZpeGVkQXJnc1swXV0gPSBkc3Q7XG4gICAgfSBlbHNlIHtcbiAgICAgIGxldCByZWNvcmQgPSBfbWF5QmVBZGRSZWNvcmQoZHN0LCBkc3RSZWNvcmRzLCBleGNsdWRlZElkeHMsIHNraXBEZXB0aCA+IDApO1xuICAgICAgaW5kZXhNYXAuc2V0KHNyYy5zZWxmSW5kZXgsIHJlY29yZC5zZWxmSW5kZXgpO1xuICAgIH1cbiAgfVxuXG4gIHJldHVybiBfb3B0aW1pemVTa2lwcyhkc3RSZWNvcmRzKTtcbn1cblxuLyoqXG4gKiAtIENvbmRpdGlvbmFsIHNraXAgb2YgMSByZWNvcmQgZm9sbG93ZWQgYnkgYW4gdW5jb25kaXRpb25hbCBza2lwIG9mIE4gYXJlIHJlcGxhY2VkIGJ5ICBhXG4gKiAgIGNvbmRpdGlvbmFsIHNraXAgb2YgTiB3aXRoIHRoZSBuZWdhdGVkIGNvbmRpdGlvbixcbiAqIC0gU2tpcHMgb2YgMCByZWNvcmRzIGFyZSByZW1vdmVkXG4gKi9cbmZ1bmN0aW9uIF9vcHRpbWl6ZVNraXBzKHNyY1JlY29yZHM6IFByb3RvUmVjb3JkW10pOiBQcm90b1JlY29yZFtdIHtcbiAgbGV0IGRzdFJlY29yZHMgPSBbXTtcbiAgbGV0IHNraXBTb3VyY2VzID0gTGlzdFdyYXBwZXIuY3JlYXRlRml4ZWRTaXplKHNyY1JlY29yZHMubGVuZ3RoKTtcbiAgbGV0IGluZGV4TWFwOiBNYXA8bnVtYmVyLCBudW1iZXI+ID0gbmV3IE1hcDxudW1iZXIsIG51bWJlcj4oKTtcblxuICBmb3IgKGxldCBwcm90b0luZGV4ID0gMDsgcHJvdG9JbmRleCA8IHNyY1JlY29yZHMubGVuZ3RoOyBwcm90b0luZGV4KyspIHtcbiAgICBsZXQgc2tpcFJlY29yZCA9IHNraXBTb3VyY2VzW3Byb3RvSW5kZXhdO1xuICAgIGlmIChpc1ByZXNlbnQoc2tpcFJlY29yZCkpIHtcbiAgICAgIHNraXBSZWNvcmQuZml4ZWRBcmdzWzBdID0gZHN0UmVjb3Jkcy5sZW5ndGg7XG4gICAgfVxuXG4gICAgbGV0IHNyYyA9IHNyY1JlY29yZHNbcHJvdG9JbmRleF07XG5cbiAgICBpZiAoc3JjLmlzU2tpcFJlY29yZCgpKSB7XG4gICAgICBpZiAoc3JjLmlzQ29uZGl0aW9uYWxTa2lwUmVjb3JkKCkgJiYgc3JjLmZpeGVkQXJnc1swXSA9PT0gcHJvdG9JbmRleCArIDIgJiZcbiAgICAgICAgICBwcm90b0luZGV4IDwgc3JjUmVjb3Jkcy5sZW5ndGggLSAxICYmXG4gICAgICAgICAgc3JjUmVjb3Jkc1twcm90b0luZGV4ICsgMV0ubW9kZSA9PT0gUmVjb3JkVHlwZS5Ta2lwUmVjb3Jkcykge1xuICAgICAgICBzcmMubW9kZSA9IHNyYy5tb2RlID09PSBSZWNvcmRUeXBlLlNraXBSZWNvcmRzSWYgPyBSZWNvcmRUeXBlLlNraXBSZWNvcmRzSWZOb3QgOlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBSZWNvcmRUeXBlLlNraXBSZWNvcmRzSWY7XG4gICAgICAgIHNyYy5maXhlZEFyZ3NbMF0gPSBzcmNSZWNvcmRzW3Byb3RvSW5kZXggKyAxXS5maXhlZEFyZ3NbMF07XG4gICAgICAgIHByb3RvSW5kZXgrKztcbiAgICAgIH1cblxuICAgICAgaWYgKHNyYy5maXhlZEFyZ3NbMF0gPiBwcm90b0luZGV4ICsgMSkge1xuICAgICAgICBsZXQgZHN0ID0gX2Nsb25lQW5kVXBkYXRlSW5kZXhlcyhzcmMsIGRzdFJlY29yZHMsIGluZGV4TWFwKTtcbiAgICAgICAgZHN0UmVjb3Jkcy5wdXNoKGRzdCk7XG4gICAgICAgIHNraXBTb3VyY2VzW2RzdC5maXhlZEFyZ3NbMF1dID0gZHN0O1xuICAgICAgfVxuXG4gICAgfSBlbHNlIHtcbiAgICAgIGxldCBkc3QgPSBfY2xvbmVBbmRVcGRhdGVJbmRleGVzKHNyYywgZHN0UmVjb3JkcywgaW5kZXhNYXApO1xuICAgICAgZHN0UmVjb3Jkcy5wdXNoKGRzdCk7XG4gICAgICBpbmRleE1hcC5zZXQoc3JjLnNlbGZJbmRleCwgZHN0LnNlbGZJbmRleCk7XG4gICAgfVxuICB9XG5cbiAgcmV0dXJuIGRzdFJlY29yZHM7XG59XG5cbi8qKlxuICogQWRkIGEgbmV3IHJlY29yZCBvciByZS11c2Ugb25lIG9mIHRoZSBleGlzdGluZyByZWNvcmRzLlxuICovXG5mdW5jdGlvbiBfbWF5QmVBZGRSZWNvcmQocmVjb3JkOiBQcm90b1JlY29yZCwgZHN0UmVjb3JkczogUHJvdG9SZWNvcmRbXSwgZXhjbHVkZWRJZHhzOiBudW1iZXJbXSxcbiAgICAgICAgICAgICAgICAgICAgICAgICBleGNsdWRlZDogYm9vbGVhbik6IFByb3RvUmVjb3JkIHtcbiAgbGV0IG1hdGNoID0gX2ZpbmRGaXJzdE1hdGNoKHJlY29yZCwgZHN0UmVjb3JkcywgZXhjbHVkZWRJZHhzKTtcblxuICBpZiAoaXNQcmVzZW50KG1hdGNoKSkge1xuICAgIGlmIChyZWNvcmQubGFzdEluQmluZGluZykge1xuICAgICAgZHN0UmVjb3Jkcy5wdXNoKF9jcmVhdGVTZWxmUmVjb3JkKHJlY29yZCwgbWF0Y2guc2VsZkluZGV4LCBkc3RSZWNvcmRzLmxlbmd0aCArIDEpKTtcbiAgICAgIG1hdGNoLnJlZmVyZW5jZWRCeVNlbGYgPSB0cnVlO1xuICAgIH0gZWxzZSB7XG4gICAgICBpZiAocmVjb3JkLmFyZ3VtZW50VG9QdXJlRnVuY3Rpb24pIHtcbiAgICAgICAgbWF0Y2guYXJndW1lbnRUb1B1cmVGdW5jdGlvbiA9IHRydWU7XG4gICAgICB9XG4gICAgfVxuXG4gICAgcmV0dXJuIG1hdGNoO1xuICB9XG5cbiAgaWYgKGV4Y2x1ZGVkKSB7XG4gICAgZXhjbHVkZWRJZHhzLnB1c2gocmVjb3JkLnNlbGZJbmRleCk7XG4gIH1cblxuICBkc3RSZWNvcmRzLnB1c2gocmVjb3JkKTtcbiAgcmV0dXJuIHJlY29yZDtcbn1cblxuLyoqXG4gKiBSZXR1cm5zIHRoZSBmaXJzdCBgUHJvdG9SZWNvcmRgIHRoYXQgbWF0Y2hlcyB0aGUgcmVjb3JkLlxuICovXG5mdW5jdGlvbiBfZmluZEZpcnN0TWF0Y2gocmVjb3JkOiBQcm90b1JlY29yZCwgZHN0UmVjb3JkczogUHJvdG9SZWNvcmRbXSxcbiAgICAgICAgICAgICAgICAgICAgICAgICBleGNsdWRlZElkeHM6IG51bWJlcltdKTogUHJvdG9SZWNvcmQge1xuICByZXR1cm4gZHN0UmVjb3Jkcy5maW5kKFxuICAgICAgLy8gVE9ETyh2aWNiKTogb3B0aW1pemUgZXhjbHVkZWRJZHhzLmluZGV4T2YgKHNvcnRlZCBhcnJheSlcbiAgICAgIHJyID0+IGV4Y2x1ZGVkSWR4cy5pbmRleE9mKHJyLnNlbGZJbmRleCkgPT0gLTEgJiYgcnIubW9kZSAhPT0gUmVjb3JkVHlwZS5EaXJlY3RpdmVMaWZlY3ljbGUgJiZcbiAgICAgICAgICAgIF9oYXZlU2FtZURpckluZGV4KHJyLCByZWNvcmQpICYmIHJyLm1vZGUgPT09IHJlY29yZC5tb2RlICYmXG4gICAgICAgICAgICBsb29zZUlkZW50aWNhbChyci5mdW5jT3JWYWx1ZSwgcmVjb3JkLmZ1bmNPclZhbHVlKSAmJlxuICAgICAgICAgICAgcnIuY29udGV4dEluZGV4ID09PSByZWNvcmQuY29udGV4dEluZGV4ICYmIGxvb3NlSWRlbnRpY2FsKHJyLm5hbWUsIHJlY29yZC5uYW1lKSAmJlxuICAgICAgICAgICAgTGlzdFdyYXBwZXIuZXF1YWxzKHJyLmFyZ3MsIHJlY29yZC5hcmdzKSk7XG59XG5cbi8qKlxuICogQ2xvbmUgdGhlIGBQcm90b1JlY29yZGAgYW5kIGNoYW5nZXMgdGhlIGluZGV4ZXMgZm9yIHRoZSBvbmVzIGluIHRoZSBkZXN0aW5hdGlvbiBhcnJheSBmb3I6XG4gKiAtIHRoZSBhcmd1bWVudHMsXG4gKiAtIHRoZSBjb250ZXh0LFxuICogLSBzZWxmXG4gKi9cbmZ1bmN0aW9uIF9jbG9uZUFuZFVwZGF0ZUluZGV4ZXMocmVjb3JkOiBQcm90b1JlY29yZCwgZHN0UmVjb3JkczogUHJvdG9SZWNvcmRbXSxcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgaW5kZXhNYXA6IE1hcDxudW1iZXIsIG51bWJlcj4pOiBQcm90b1JlY29yZCB7XG4gIGxldCBhcmdzID0gcmVjb3JkLmFyZ3MubWFwKHNyYyA9PiBfc3JjVG9Ec3RTZWxmSW5kZXgoaW5kZXhNYXAsIHNyYykpO1xuICBsZXQgY29udGV4dEluZGV4ID0gX3NyY1RvRHN0U2VsZkluZGV4KGluZGV4TWFwLCByZWNvcmQuY29udGV4dEluZGV4KTtcbiAgbGV0IHNlbGZJbmRleCA9IGRzdFJlY29yZHMubGVuZ3RoICsgMTtcblxuICByZXR1cm4gbmV3IFByb3RvUmVjb3JkKHJlY29yZC5tb2RlLCByZWNvcmQubmFtZSwgcmVjb3JkLmZ1bmNPclZhbHVlLCBhcmdzLCByZWNvcmQuZml4ZWRBcmdzLFxuICAgICAgICAgICAgICAgICAgICAgICAgIGNvbnRleHRJbmRleCwgcmVjb3JkLmRpcmVjdGl2ZUluZGV4LCBzZWxmSW5kZXgsIHJlY29yZC5iaW5kaW5nUmVjb3JkLFxuICAgICAgICAgICAgICAgICAgICAgICAgIHJlY29yZC5sYXN0SW5CaW5kaW5nLCByZWNvcmQubGFzdEluRGlyZWN0aXZlLFxuICAgICAgICAgICAgICAgICAgICAgICAgIHJlY29yZC5hcmd1bWVudFRvUHVyZUZ1bmN0aW9uLCByZWNvcmQucmVmZXJlbmNlZEJ5U2VsZixcbiAgICAgICAgICAgICAgICAgICAgICAgICByZWNvcmQucHJvcGVydHlCaW5kaW5nSW5kZXgpO1xufVxuXG4vKipcbiAqIFJldHVybnMgdGhlIGluZGV4IGluIHRoZSBkZXN0aW5hdGlvbiBhcnJheSBjb3JyZXNwb25kaW5nIHRvIHRoZSBpbmRleCBpbiB0aGUgc3JjIGFycmF5LlxuICogV2hlbiB0aGUgZWxlbWVudCBpcyBub3QgcHJlc2VudCBpbiB0aGUgZGVzdGluYXRpb24gYXJyYXksIHJldHVybiB0aGUgc291cmNlIGluZGV4LlxuICovXG5mdW5jdGlvbiBfc3JjVG9Ec3RTZWxmSW5kZXgoaW5kZXhNYXA6IE1hcDxudW1iZXIsIG51bWJlcj4sIHNyY0lkeDogbnVtYmVyKTogbnVtYmVyIHtcbiAgdmFyIGRzdElkeCA9IGluZGV4TWFwLmdldChzcmNJZHgpO1xuICByZXR1cm4gaXNQcmVzZW50KGRzdElkeCkgPyBkc3RJZHggOiBzcmNJZHg7XG59XG5cbmZ1bmN0aW9uIF9jcmVhdGVTZWxmUmVjb3JkKHI6IFByb3RvUmVjb3JkLCBjb250ZXh0SW5kZXg6IG51bWJlciwgc2VsZkluZGV4OiBudW1iZXIpOiBQcm90b1JlY29yZCB7XG4gIHJldHVybiBuZXcgUHJvdG9SZWNvcmQoUmVjb3JkVHlwZS5TZWxmLCBcInNlbGZcIiwgbnVsbCwgW10sIHIuZml4ZWRBcmdzLCBjb250ZXh0SW5kZXgsXG4gICAgICAgICAgICAgICAgICAgICAgICAgci5kaXJlY3RpdmVJbmRleCwgc2VsZkluZGV4LCByLmJpbmRpbmdSZWNvcmQsIHIubGFzdEluQmluZGluZyxcbiAgICAgICAgICAgICAgICAgICAgICAgICByLmxhc3RJbkRpcmVjdGl2ZSwgZmFsc2UsIGZhbHNlLCByLnByb3BlcnR5QmluZGluZ0luZGV4KTtcbn1cblxuZnVuY3Rpb24gX2hhdmVTYW1lRGlySW5kZXgoYTogUHJvdG9SZWNvcmQsIGI6IFByb3RvUmVjb3JkKTogYm9vbGVhbiB7XG4gIHZhciBkaTEgPSBpc0JsYW5rKGEuZGlyZWN0aXZlSW5kZXgpID8gbnVsbCA6IGEuZGlyZWN0aXZlSW5kZXguZGlyZWN0aXZlSW5kZXg7XG4gIHZhciBlaTEgPSBpc0JsYW5rKGEuZGlyZWN0aXZlSW5kZXgpID8gbnVsbCA6IGEuZGlyZWN0aXZlSW5kZXguZWxlbWVudEluZGV4O1xuXG4gIHZhciBkaTIgPSBpc0JsYW5rKGIuZGlyZWN0aXZlSW5kZXgpID8gbnVsbCA6IGIuZGlyZWN0aXZlSW5kZXguZGlyZWN0aXZlSW5kZXg7XG4gIHZhciBlaTIgPSBpc0JsYW5rKGIuZGlyZWN0aXZlSW5kZXgpID8gbnVsbCA6IGIuZGlyZWN0aXZlSW5kZXguZWxlbWVudEluZGV4O1xuXG4gIHJldHVybiBkaTEgPT09IGRpMiAmJiBlaTEgPT09IGVpMjtcbn1cbiJdfQ==