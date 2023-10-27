import { merge } from 'webpack-merge';
import { baseConfig, fileManagerPlugin } from './webpack.config.js';

export default merge(baseConfig, {
    mode: 'development',
    devtool: 'inline-source-map',
    plugins: [
        fileManagerPlugin
    ],
});