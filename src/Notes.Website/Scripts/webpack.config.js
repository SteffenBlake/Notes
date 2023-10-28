import * as path from 'path';

import ESLintPlugin from 'eslint-webpack-plugin';
import HtmlBundlerPlugin from 'html-bundler-webpack-plugin';
import FileManagerPlugin from 'filemanager-webpack-plugin';

import { fileURLToPath } from 'url';
import { dirname } from 'path';
const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

export default {
    plugins: [
        new ESLintPlugin(),
        new HtmlBundlerPlugin({
            entry: {
                index: {
                    import: 'src/views/_layout.ejs',
                    data: {
                        page: 'home',
                        headerEnabled: true
                    }, 
                },
                login: {
                    import: 'src/views/_layout.ejs',
                    data: {
                        page: 'login',
                        headerEnabled: false
                    },
                },
                denied: {
                    import: 'src/views/_layout.ejs',
                    data: {
                        page: 'denied',
                        headerEnabled: false
                    },
                },
            },
            js: {
                filename: 'scripts/[name].[contenthash:8].js',
            },
            css: {
                filename: 'styles/[name].[contenthash:8].css',
            },
            preprocessor: 'ejs',
            minify: 'auto',
            //integrity: 'auto', // Seems to be bugged?

        }),
        new FileManagerPlugin({
            events: {
                onStart: {
                    delete: [
                        {
                            source: '../wwwroot/notes/*',
                            options: {
                                force: true
                            }
                        },
                        {
                            source: '../pages/*',
                            options: {
                                force: true
                            }
                        }
                    ]
                },
                onEnd: {
                    copy: [
                        {
                            source: path.resolve(__dirname, 'dist/notes/scripts/*'),
                            destination: path.resolve(__dirname, '../wwwroot/notes/scripts/')
                        },
                        {
                            source: path.resolve(__dirname, 'dist/notes/styles/*'),
                            destination: path.resolve(__dirname, '../wwwroot/notes/styles/')
                        },
                        {
                            source: path.resolve(__dirname, 'dist/notes/assets/**/*'),
                            destination: path.resolve(__dirname, '../wwwroot/notes/assets/')
                        },
                        {
                            source: path.resolve(__dirname, 'dist/notes/*.html'),
                            destination: path.resolve(__dirname, '../pages')
                        }
                    ]
                }
            }
        })
    ],
    output: {
        filename: 'scripts/notes/[name].[chunkhash].js',
        path: path.resolve(__dirname, 'dist/notes/'),
        publicPath: '/notes/',
        crossOriginLoading: "anonymous",
        clean: true
    },
    module: {
        rules: [
            {
                test: /\.(css|sass|scss)$/,
                use: ['css-loader', 'sass-loader'],
            },
            {
                test: /\.(ico|png|jp?g|webp|svg)$/,
                type: 'asset/resource',
                generator: {
                    filename: 'assets/img/[name].[hash:8][ext][query]',
                },
            },
        ],
    }
};