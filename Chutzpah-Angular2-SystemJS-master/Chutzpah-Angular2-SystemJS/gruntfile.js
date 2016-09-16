/// <binding BeforeBuild='copy:main, copy:tests, sass:compile' />
module.exports = function (grunt) {
    grunt.initConfig({
        copy: {
            main: {
                files: [
                    { expand: true, flatten: true, src: 'node_modules/es6-shim/es6-shim.js', dest: 'wwwroot/scripts/' },
                    { expand: true, flatten: true, src: 'node_modules/systemjs/dist/system-polyfills.src.js', dest: 'wwwroot/scripts/' },
                    { expand: true, flatten: true, src: 'node_modules/angular2/bundles/angular2-polyfills.js', dest: 'wwwroot/scripts/' },
                    { expand: true, flatten: true, src: 'node_modules/systemjs/dist/system.src.js', dest: 'wwwroot/scripts/' },
                    { expand: true, flatten: true, src: 'node_modules/rxjs/bundles/Rx.js', dest: 'wwwroot/scripts/' },
                    { expand: true, flatten: true, src: 'node_modules/angular2/bundles/angular2.dev.js', dest: 'wwwroot/scripts/' },
                    { expand: true, flatten: true, src: 'app/scripts/boot.js', dest: 'wwwroot/scripts/' },
                    { expand: true, flatten: true, src: 'app/scripts/node.component.js', dest: 'wwwroot/scripts/' },
                    { expand: true, flatten: true, src: 'app/views/*.html', dest: 'wwwroot/views/' },
                { expand: true, flatten: true, src: 'app/scripts/node.component.spec.js', dest: 'wwwroot/scripts/' }
                ]
            }
        }
    });

    grunt.loadNpmTasks('grunt-contrib-copy');
};