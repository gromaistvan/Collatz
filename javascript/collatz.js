'use strict';

const http = require('http');
const fs = require('fs');

function createImage(max, numbers) {
    const fullHeight = 600;
    const barWidth = 12;
    const top = 10, gap = 10;
    const fullWidth = gap + numbers.length * barWidth;
    const bars = [];
    for (let i = 0; i < numbers.length; i++) {
        const height = numbers[i] / max * (fullHeight - top);
        bars.push(`            <rect x="${gap + i * barWidth}" y="${fullHeight + top - height}" width="${barWidth}" height="${height}"/>`);
    }
    for (let i = 0; i < numbers.length; i++) {
        const height = numbers[i] / max * (fullHeight - top);
        bars.push(`            <text x="${gap + (i + .5) * barWidth}" y="${fullHeight + top - height - 2}">${numbers[i]}</text>`);
    }
    const rects = bars.join('');
    return `
        <svg xmlns="http://www.w3.org/2000/svg" version="1.1" height="${fullHeight}" width="${fullWidth}">
            ${rects}
        </svg>
`.trim();
}

function createHtml(number, image) {
    return `
<!DOCTYPE html>
<html lang="en">
    <head>
        <meta charset="utf-8">
        <meta http-equiv="X-UA-Compatible" content="IE=edge">
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <title>Collatz sequence of ${number}</title>
        <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-beta/css/bootstrap.min.css" integrity="sha384-/Y6pD6FV/Vv2HJnA6t+vslU6fwYXjCFtcEpHbNJ0lyAFsXTsjBbfaDjzALeQsN6M" crossorigin="anonymous">
        <style>

rect { fill: coral; stroke: white }
text { font-size: 10pt; text-anchor: middle }

        </style>
    </head>
    <body class="container-fluid">
        <h1>
            <a href="https://en.wikipedia.org/wiki/Collatz_conjecture" target="_blank">Collatz sequence of ${number}</a>
        </h1>
        ${image}
        <br>
        <a id="previous" class="btn btn-primary" href="./${number-1}" accesskey="a">Previous</a>
        <a id="next" class="btn btn-primary" href="./${number + 1}" accesskey="s">Next</a>

        <script src="https://code.jquery.com/jquery-3.2.1.slim.min.js" integrity="sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN" crossorigin="anonymous"></script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.11.0/umd/popper.min.js" integrity="sha384-b/U6ypiBEHpOf/4+1nzFpr53nxSS+GLCkfwBdFNTxtclqqenISfwAzpKaMNFNmj4" crossorigin="anonymous"></script>
        <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-beta/js/bootstrap.min.js" integrity="sha384-h0AbiXch4ZDo7tp9hKZ4TsHbi047NrKGLO3SEJAg45jXxnGIfYzk4Si90RDIqNm1" crossorigin="anonymous"></script>
        <script>

$(document).keyup(function(event) {
     var keyCode = event.which || event.keyCode
    if (event.keyCode == 37) {
        $('#previous')[0].click();
        return false;
    }
    if (keyCode == 39) {
        $('#next')[0].click();
        return false;
    }
    return true;
});

        </script>
    </body>
</html>
`.trim();
}

exports.createImage = createImage;
exports.createHtml = createHtml;

function findCollatz(number, res) {
    if (typeof number != 'number' || number <= 0) {
        res.statusCode = 404;
        res.end();
        return;
    }
    var ending = '';
    const batchSize = 100000;
    const fileIdx = `000000${(number / batchSize) >> 0}`.slice(-6);
    var lineIdx = (number % batchSize);
    if (lineIdx === 0) lineIdx = batchSize;
    fs.createReadStream(`collatz.${fileIdx}.txt`)
        .on('data', chunk => {
            if (lineIdx <= 0) return;
            var lines = (ending + chunk).split('\n');
            if (lineIdx > lines.length) {
                lineIdx -= lines.length - 1;
                ending = lines[lines.length - 1];
            } else if (lines.length > lineIdx) {
                const data = lines[lineIdx - 1].split(':');
                const items = data[1].split(/\\|\//);
                const maxIdx = items.findIndex(value => /\(\d+\)/.test(value));
                let max = items[maxIdx];
                items[maxIdx] = max = max.substring(1, max.length - 1);
                res.writeHeader(200, { 'Content-Type': 'text/html' });
                res.end(createHtml(number, createImage(max, items)));
                lineIdx = 0;
            } else {
                ending += chunk;
            }
        })
        .on('end', () => {
            if (lineIdx <= 0) return;
            res.statusCode = 404;
            res.end();
        });
}

http.createServer((req, res) => {
        var number = parseInt(req.url.slice(1));
        findCollatz(number, res);
    })
    .listen(process.env['PORT'] || 4214);

