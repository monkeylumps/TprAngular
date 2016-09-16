System.config({
    packages: {
        app: {
            format: 'register',
            defaultExtension: 'js'
        }
    }
});
//System.import('app/app.component.spec')
//    .then(null, console.error.bind(console));

//System.config({
//    baseURL: "/",
//    defaultJSExtensions: true,
//    transpiler: "typescript",
//    typescriptOptions: {
//        "module": "system",
//        "emitDecoratorMetadata": true,
//        "experimentalDecorators": true
//    },
//    paths: {
//        "app": "app"
//    },

//    packages: {
//        "app": {
//            "main": "boot",
//            "defaultExtension": "ts",
//            "modules": {
//                "*.ts": {
//                    "loader": "ts"
//                }
//            }
//        }
//    },
//});