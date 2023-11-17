import { getPartials } from './get-partials.js';
import { getEntries } from './get-entries.js';
import { default as helpers } from '../helpers/_helpers.js';

export default {
    entry: getEntries(),
    js: {
        filename: 'scripts/[name].[contenthash:8].js',
    },
    css: {
        filename: 'styles/[name].[contenthash:8].css',
    },
    preprocessor: 'handlebars',
    preprocessorOptions: {
        partials: getPartials(),
        helpers
    },
    minify: 'auto',
    integrity: {
        enabled: true,
        hashFunctions: 'sha384',
    },
}