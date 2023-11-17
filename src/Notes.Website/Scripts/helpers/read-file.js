import { readFileSync as readFile } from 'fs';

export default function(path) {
    return readFile(path);
} 