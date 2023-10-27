import { merge } from 'webpack-merge';
import { baseConfig, fileManagerPlugin } from './webpack.config.js';

import { SubresourceIntegrityPlugin } from 'webpack-subresource-integrity';

export default merge(baseConfig, {
    mode: 'production',

    plugins: [
        new SubresourceIntegrityPlugin({
            enabled: true,
        }),

        fileManagerPlugin
    ],
});