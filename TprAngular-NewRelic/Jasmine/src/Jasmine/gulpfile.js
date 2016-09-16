/// <binding AfterBuild='build' Clean='clean' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    path = require('path'),
    eventStream = require('event-stream'),
    inlineNg2Template = require('gulp-inline-ng2-template'),
    sourcemaps = require('gulp-sourcemaps'),
    project = require("./project.json"),
    ts = require('gulp-typescript');

var paths = {
    webroot: "./wwwroot/"
};

var config = {
    libBase: 'node_modules',
    lib: [
        require.resolve('systemjs/dist/system.js'),
        require.resolve('systemjs/dist/system.src.js'),
        require.resolve('systemjs/dist/system-polyfills.js'),
        require.resolve('angular2/bundles/angular2-polyfills.js'),
        require.resolve('angular2/bundles/angular2.dev.js'),
        require.resolve('angular2/bundles/testing.dev.js'),
        require.resolve('angular2/bundles/router.dev.js'),
        require.resolve('angular2/bundles/http.js'),
        require.resolve('rxjs/bundles/Rx.js'),
        require.resolve('jasmine-core/lib/jasmine-core/jasmine.css'),
        require.resolve('typescript/lib/typescript.js'),
        require.resolve('jasmine-core/lib/jasmine-core/jasmine.js'),
        require.resolve('jasmine-core/lib/jasmine-core/boot.js'),
        require.resolve('jasmine-core/lib/jasmine-core/jasmine-html.js')
    ]
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";

gulp.task('build.lib', function () {
    return gulp.src(config.lib, { base: config.libBase })
        .pipe(gulp.dest(paths.webroot + 'lib'));
});

gulp.task('build-prod', function () {
    var tsProject = ts.createProject('./tsconfig.json', { typescript: require('typescript') });
    var tsSrcInlined = gulp.src([webroot + '**/*.ts'], { base: webroot })
        .pipe(inlineNg2Template({ base: webroot }));
    return eventStream.merge(tsSrcInlined, gulp.src('Typings/**/*.ts'))
        .pipe(sourcemaps.init())
        .pipe(typescript(tsProject))
        .pipe(sourcemaps.write())
        .pipe(gulp.dest(webroot));
});

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task('watch', function () {
    gulp.watch(['./wwwroot/**/*.ts'], ['build-prod']);
});

gulp.task("min", ["min:js", "min:css"]);