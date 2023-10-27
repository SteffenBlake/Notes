import * as path from 'path';
import devUtils from '@ckeditor/ckeditor5-dev-utils';
const { styles } = devUtils;

import HtmlWebpackPlugin from 'html-webpack-plugin';
import MiniCssExtractPlugin from 'mini-css-extract-plugin';
import CssMinimizerPlugin from 'css-minimizer-webpack-plugin';
import FileManagerPlugin from 'filemanager-webpack-plugin';

import { fileURLToPath } from 'url';
import { dirname } from 'path';
const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

import { createRequire } from 'node:module';
const require = createRequire(import.meta.url);

const entry =
{
    index: "./src/pages/index/index.js",
    login: "./src/pages/login/login.js",
    denied: "./src/pages/denied/denied.js",
};

let webpackPlugins = Object.keys(entry).map(e =>
    new HtmlWebpackPlugin({
        title: `Notes-${e}`,
        template: `./src/pages/${e}/${e}.ejs`,
        favicon: './resources/favicon.ico',
        filename: `${e}.html`,
        chunks: [e, 'commons'],
        hash: true
    })
);

var baseConfig = {
    entry,
    plugins: [
        ...webpackPlugins,
        new MiniCssExtractPlugin({
            filename: "styles/[name].css",
            chunkFilename: "styles/[id].css"
        }),
    ],
    output: {
        filename: 'scripts/[name].[chunkhash].js',
        path: path.resolve(__dirname, 'dist'),
        publicPath: '/',
        crossOriginLoading: "anonymous",
        clean: true
    },
    module: {
        rules: [
            {
                test: /\.svg$/i,
                exclude: /node_modules/,
                use: ['raw-loader'],
            },
            {
                test: /\.css$/i,
                exclude: /node_modules/,
                use: [MiniCssExtractPlugin.loader, "css-loader"],
                sideEffects: true,
            },
            {
                test: /\.s[ac]ss$/i,
                exclude: /node_modules/,
                use: [ MiniCssExtractPlugin.loader,"css-loader","sass-loader" ],
            },
            {
                test: /ckeditor5-[^/\\]+[/\\]theme[/\\]icons[/\\][^/\\]+\.svg$/,
                use: ['raw-loader']
            },
            {
                test: /ckeditor5-[^/\\]+[/\\]theme[/\\].+\.css$/,
                use: [
                    {
                        loader: MiniCssExtractPlugin.loader
                    },
                    'css-loader',
                    {
                        loader: 'postcss-loader',
                        options: {
                            postcssOptions: styles.getPostCssConfig({
                                themeImporter: {
                                    themePath: require.resolve('@ckeditor/ckeditor5-theme-lark')
                                },
                                minify: true
                            })
                        }
                    }
                ]
            }
        ],
    },
    // optimization automates minification for prod mode
    optimization: {
        minimizer: [
            new CssMinimizerPlugin(),
        ],
        // splits css and js up so common file can exist to optimize
        splitChunks: {
            chunks: "initial",
            name: 'commons'
        }
    },
};

// fileManager is the same config for dev/prod
// But we have some middleware plugins for prod
// So we export fileManager seperately so it can
// be injected last in both enviros
var fileManagerPlugin = new FileManagerPlugin({
    events: {
        onStart: {
            delete: [
                {
                    source: '../wwwroot/*',
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
                    source: path.resolve(__dirname, 'dist/scripts/*'),
                    destination: path.resolve(__dirname, '../wwwroot/scripts/')
                },
                {
                    source: path.resolve(__dirname, 'dist/styles/*'),
                    destination: path.resolve(__dirname, '../wwwroot/styles/')
                },
                ,
                {
                    source: path.resolve(__dirname, 'dist/*.html'),
                    destination: path.resolve(__dirname, '../pages')
                }
            ]
        }
    }
});


export { baseConfig, fileManagerPlugin };