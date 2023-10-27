import baseConfig from './webpack.config.js';

baseConfig.mode = 'development';
baseConfig.devtool = 'inline-source-map';

export default baseConfig;