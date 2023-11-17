import { readdirSync as readDir } from 'fs';

export function getPartials(path = "views") {
    var dirs = readDir(path, { 
            withFileTypes: true,
            recursive: true
        })
        .filter(d => d.isDirectory())
        .map(d => d.path + "\\" + d.name);
    dirs.push(path);
    return dirs;
}