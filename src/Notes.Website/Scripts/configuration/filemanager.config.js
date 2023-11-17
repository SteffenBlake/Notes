import resolvePath from "./resolve-path.js";

export default {
    events: {
        onStart: {
            delete: [
                {
                    source: '../../wwwroot/notes/*',
                    options: {
                        force: true
                    }
                },
                {
                    source: '../../pages/*',
                    options: {
                        force: true
                    }
                }
            ]
        },
        onEnd: {
            copy: [
                {
                    source: resolvePath('../dist/notes/scripts/*'),
                    destination: resolvePath('../../wwwroot/notes/scripts/')
                },
                {
                    source: resolvePath('../dist/notes/styles/*'),
                    destination: resolvePath('../../wwwroot/notes/styles/')
                },
                {
                    source: resolvePath('../dist/notes/assets/**/*'),
                    destination: resolvePath('../../wwwroot/notes/assets/')
                },
                {
                    source: resolvePath('../dist/notes/*.html'),
                    destination: resolvePath('../../pages')
                }
            ]
        }
    }
};