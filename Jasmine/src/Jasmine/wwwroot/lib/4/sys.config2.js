System.config({
    packages: {
        app: {
            format: 'register',
            defaultExtension: 'js'
        }
    }
});
System.import('app/app.component.spec')
    .then(null, console.error.bind(console));