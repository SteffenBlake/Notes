import { fileURLToPath } from 'url';
import { dirname, resolve } from 'path';

export default function(path) {
    return resolve(dirname(fileURLToPath(import.meta.url)), path);
}