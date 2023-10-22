const path = require('path');

const HtmlWebpackPlugin = require('html-webpack-plugin');
const CspHtmlWebpackPlugin = require('csp-html-webpack-plugin');

const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");

const FileManagerPlugin = require('filemanager-webpack-plugin');

const dist = path.resolve(__dirname, 'dist');

module.exports = {
    entry: './src/index.js',
    output: {
        filename: 'notes.js',
        path: path.resolve(__dirname, 'dist'),
    },
    module: {
        rules: [
            {
                test: /\.css$/i,
                use: [MiniCssExtractPlugin.loader, "css-loader"],
            },
        ],
    },
    plugins: [
        new HtmlWebpackPlugin({
            title: 'Notes',
            template: './src/index.ejs'
        }),
        new CspHtmlWebpackPlugin({
            'script-src': '',
            'style-src': ''
        }),
        new MiniCssExtractPlugin(),
        new FileManagerPlugin({
            events: {
                onEnd: {
                    copy: [
                        {
                            source: path.resolve(__dirname, 'dist/*'),
                            destination: path.resolve(__dirname, '../wwwroot')
                        }
                    ]
                }
            }
        })
    ],
    optimization: {
        minimizer: [
            new CssMinimizerPlugin(),
        ],
    },
};