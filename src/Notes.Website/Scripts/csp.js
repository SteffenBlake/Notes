const { hashElement } = require('folder-hash');

(async () => {
    foo = await main();
})();

async function main() {
    const jsHashSet = await hashElement('dist', {
        files: { include: ['*.jss'] },
        algo: 'sha256',
        encoding: 'base64'
    });
    const cssHashSet = await hashElement('dist', {
        files: { include: ['*.css'] },
        algo: 'sha256',
        encoding: 'base64'
    });

    const jsHashes = pullHashes(jsHashSet);
    const cssHashes = pullHashes(cssHashSet);

    console.log(jsHashes.toString());
    console.log(cssHashes.toString());
}

function pullHashes(hashSet) {
    if (!hashSet.hasOwnProperty('children')) {
        return [hashSet.hash];
    }
    var descendants = [];
    for (var n = 0; n < hashSet.children.length; n++) {
        var childHashes = pullHashes(hashSet.children[n]);
        descendants = descendants.concat(childHashes);
    }

    return descendants;
}