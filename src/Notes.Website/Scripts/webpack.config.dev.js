import baseConfig from './configuration/webpack.config.js';

baseConfig.mode = 'development';
baseConfig.devtool = 'inline-source-map';

export default baseConfig;