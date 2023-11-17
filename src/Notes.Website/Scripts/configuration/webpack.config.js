import ESLintPlugin from 'eslint-webpack-plugin';

import HtmlBundlerPlugin from 'html-bundler-webpack-plugin';
import HtmlBundlerConfig from './html-bundler.config.js';

import FileManagerPlugin from 'filemanager-webpack-plugin';
import FileManagerConfig from './filemanager.config.js';

import resolvePath from "./resolve-path.js";

export default {
    plugins: [
        new ESLintPlugin(),
        new HtmlBundlerPlugin(HtmlBundlerConfig),
        new FileManagerPlugin(FileManagerConfig)
    ],
    output: {
        filename: 'scripts/notes/[name].[chunkhash].js',
        path: resolvePath('../dist/notes/'),
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