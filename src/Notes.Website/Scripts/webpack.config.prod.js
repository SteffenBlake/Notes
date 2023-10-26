const { merge } = require('webpack-merge');
const base = require('./webpack.config.js');
const path = require('path');

const HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const { SubresourceIntegrityPlugin } = require('webpack-subresource-integrity');
const FileManagerPlugin = require('filemanager-webpack-plugin');

module.exports = merge(base, {
    mode: 'production',
    plugins: [
        new HtmlWebpackPlugin({
            title: 'Notes',
            template: './src/index.ejs'
        }),
        new MiniCssExtractPlugin({
            filename: "[name].css",
            chunkFilename: "[id].css"
        }),
        new SubresourceIntegrityPlugin({
            enabled: true,
        }),
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
});