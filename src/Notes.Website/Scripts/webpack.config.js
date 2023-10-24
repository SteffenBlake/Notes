const path = require('path');
const { styles } = require('@ckeditor/ckeditor5-dev-utils');

const HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");
const { SubresourceIntegrityPlugin } = require('webpack-subresource-integrity');
const FileManagerPlugin = require('filemanager-webpack-plugin');

module.exports = {
    entry: './src/index.js',
    output: {
        filename: 'notes.js',
        path: path.resolve(__dirname, 'dist'),
        publicPath: '/',
        crossOriginLoading: "anonymous",
    },
    module: {
        rules: [
            {
                test: /\.svg$/i,
                use: ['raw-loader'],
                
            },
            {
                test: /\.css$/i,
                use: [MiniCssExtractPlugin.loader, "css-loader"],
                sideEffects: true,
            },
            //{
            //    test: /ckeditor5-[^/\\]+[/\\]theme[/\\].+\.css$/,

            //    use: [
            //        {
            //            loader: MiniCssExtractPlugin.loader,
            //        },
            //        'css-loader',
            //        {
            //            loader: 'postcss-loader',
            //            options: {
            //                postcssOptions: styles.getPostCssConfig({
            //                    themeImporter: {
            //                        themePath: require.resolve('@ckeditor/ckeditor5-theme-lark')
            //                    },
            //                    minify: true
            //                })
            //            }
            //        }
            //    ]
            //},
            {
                test: /\.s[ac]ss$/i,
                use: [ MiniCssExtractPlugin.loader,"css-loader","sass-loader" ],
            },
        ],
    },
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
    optimization: {
        minimizer: [
            new CssMinimizerPlugin(),
        ],
    },
};