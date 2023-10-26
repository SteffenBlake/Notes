const path = require('path');
const { styles } = require('@ckeditor/ckeditor5-dev-utils');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");

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
    optimization: {
        minimizer: [
            new CssMinimizerPlugin(),
        ],
    },
};