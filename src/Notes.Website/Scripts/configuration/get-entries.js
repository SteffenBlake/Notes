import { readdirSync as readDir } from 'fs';

export function getEntries(path = "views", suffix = ".data.cjs") {
    var dataFiles = readDir(path, 
        { 
            withFileTypes: "true",
            recursive: true
        })
        .filter(d => d.isFile() && d.name.endsWith(suffix));

    var entries = {};
    for (const dataFile of dataFiles) {
        var name = dataFile.name.replace(suffix, '');
        var index = name == 'home' ? 'index' : name;
        entries[index] = {
            import: `${dataFile.path}\\${name}.hbs`,
            data: `${dataFile.path}\\${dataFile.name}`
        }
    }
    return entries;
}