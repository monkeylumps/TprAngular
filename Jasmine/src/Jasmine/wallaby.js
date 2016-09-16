module.exports = function (wallaby) {
    return {
        files: [
            { pattern: '*/lib/1/*.js', instrument: false },
            { pattern: '*/lib/2/*.js', instrument: false },
            { pattern: '*/lib/3/*.js', instrument: false },
            { pattern: '*/lib/4/*.js', instrument: false },
            { pattern: '*/app/*.ts', load: false },
            { pattern: 'wwwroot/*.html', load: false },
            { pattern: '*/app/*.spec.ts', ignore: true }
        ],

        tests: [
            { pattern: '*/app/*.spec.ts', load: false }
        ],

        //env: {
        //    runner: "node_modules/karma-phantomjs2-launcher/node_modules/phantomjs2-ext/bin/phantomjs"
        //},

        //compilers: {
        //    'app/*/*.ts': wallaby.compilers.typeScript({
        //        "module": 'system', // or amd
        //        "emitDecoratorMetadata": true,
        //        "experimentalDecorators": true,
        //        "noImplicitAny": false
        //    })
        //},
        //compilers: {
        //    '*/app/*.ts': wallaby.compilers.typeScript({
        //        "module": 'system', // or amd
        //        "emitDecoratorMetadata": true,
        //        "experimentalDecorators": true,
        //        "noImplicitAny": false
        //    })
        //},

        bootstrap: function (wallaby) {
            wallaby.delayStart();

            System.config({
                packages: {
                    'app': {
                        defaultExtension: 'js'
                    }
                }
                // `scriptLoad: true` needs to be used for inline error messages,
                // but `scriptLoad: true` for 'app/*' breaks loading html.
                // so somehow `scriptLoad: false` needs to be set for html, not sure how to do it (tried 'app/*.html' - no luck)
               ,
                meta: {
                    'app/*spec.js': {
                        scriptLoad: true
                    }
                }
            });

            var promises = [];
            for (var i = 0, len = wallaby.tests.length; i < len; i++) {
                promises.push(System['import'](wallaby.tests[i].replace(/\.js$/, '')));
            }

            Promise.all(promises).then(function () {
                wallaby.start();
            });
        }
    };
};