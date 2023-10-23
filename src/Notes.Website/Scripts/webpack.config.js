const path = require('path');

const HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");

module.exports = {
    entry: './src/index.js',
    output: {
        filename: 'notes.js',
        path: path.resolve(__dirname, 'dist'),
        publicPath: '/'
    },
    module: {
        rules: [
            {
                test: /\.css$/i,
                include: path.resolve(__dirname, 'src/notes.css'),
                use: [MiniCssExtractPlugin.loader, "css-loader"],
                sideEffects: true,
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
        })
    ],
    optimization: {
        minimizer: [
            new CssMinimizerPlugin(),
        ],
    },
};