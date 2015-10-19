var Imap = require('imap'),
    inspect = require('util').inspect,
    assert = require('assert');
var fs = require('fs');
var imap = new Imap({
    user: 'phuong.gtoken@gmail.com',
    password: 'gtoken123',
    host: 'imap.gmail.com',
    port: 993,
    tls: true
});

function openInbox(cb) {
    imap.openBox('INBOX', false, cb);
}

var arrContentRecruitPackage = ['New Recruit', '10 GToken', '9.99 USD', 'PayPal'];
var arrContentSkilledPackage = ['Skilled One', '20 GToken', '19.99 USD', 'PayPal'];
var arrContentKingPackage = ['King Club', '100 GToken', '99.99 USD', 'PayPal'];

// Function to test New Recruit Package Invoice
fs.readFile('../TestData/ID-Invoice/NewRecruit.txt', 'utf8', function(err, data) {
    if (err) throw err;
    imap.once('ready', function() {
        var buffer = '',
            count = 0;
        openInbox(function(err, box) {
            if (err) throw err;
            imap.search([
                ['BODY', data]
            ], function(err, results) {
                if (err) throw err;
                var f = imap.fetch(results, {
                    bodies: ['TEXT']
                });
                f.on('message', function(msg, seqno) {
                    console.log('Message #%d with ID invoice ' +  data, seqno);
                    var prefix = '(#' + seqno + ') ';
                    msg.on('body', function(stream, info) {
                        stream.on('data', function(chunk) {
                            count += chunk.length;
                            buffer += chunk.toString('utf8');
                        });
                        stream.once('end', function() {
                            if (info.which === 'TEXT')
                                console.log(prefix + 'Load body [%s] Finished', inspect(info.which));
                        });
                    });
                });
                f.once('error', function(err) {
                    console.log('Fetch error: ' + err);
                });
                f.once('end', function() {
                    console.log('Done fetching all messages!');
                    for (var i = arrContentRecruitPackage.length - 1; i >= 0; i--) {
                        if (buffer.indexOf(arrContentRecruitPackage[i]) > -1) {
                            assert.ok(true, arrContentRecruitPackage[i] + ' found');
                        } else {
                            assert.ok(false, arrContentRecruitPackage[i] + ' not found');
                        };
                    };
                    imap.end();
                });
            });
        });
    });
});

// Function to test New Recruit Package Invoice
fs.readFile('../TestData/ID-Invoice/SkilledOne.txt', 'utf8', function(err, data) {
    if (err) throw err;
    imap.once('ready', function() {
        var buffer = '',
            count = 0;
        openInbox(function(err, box) {
            if (err) throw err;
            imap.search([
                ['BODY', data]
            ], function(err, results) {
                if (err) throw err;
                var f = imap.fetch(results, {
                    bodies: ['TEXT']
                });
                f.on('message', function(msg, seqno) {
                    console.log('Message #%d with ID invoice ' +  data, seqno);
                    var prefix = '(#' + seqno + ') ';
                    msg.on('body', function(stream, info) {
                        stream.on('data', function(chunk) {
                            count += chunk.length;
                            buffer += chunk.toString('utf8');
                        });
                        stream.once('end', function() {
                            if (info.which === 'TEXT')
                                console.log(prefix + 'Load body [%s] Finished', inspect(info.which));
                        });
                    });
                });
                f.once('error', function(err) {
                    console.log('Fetch error: ' + err);
                });
                f.once('end', function() {
                    console.log('Done fetching all messages!');
                    for (var i = arrContentSkilledPackage.length - 1; i >= 0; i--) {
                        if (buffer.indexOf(arrContentSkilledPackage[i]) > -1) {
                            assert.ok(true, arrContentSkilledPackage[i] + ' found');
                        } else {
                            assert.ok(false, arrContentSkilledPackage[i] + ' not found');
                        };
                    };
                    imap.end();
                });
            });
        });
    });
});

// Function to test King Club Package Invoice
fs.readFile('../TestData/ID-Invoice/KingClub.txt', 'utf8', function(err, data) {
    imap.once('ready', function() {
        var buffer = '',
            count = 0;
        openInbox(function(err, box) {
            if (err) throw err;
            imap.search([
                ['BODY', data]
            ], function(err, results) {
                if (err) throw err;
                var f = imap.fetch(results, {
                    bodies: ['TEXT']
                });
                f.on('message', function(msg, seqno) {
                    console.log('Message #%d with ID invoice ' +  data, seqno);
                    var prefix = '(#' + seqno + ') ';
                    msg.on('body', function(stream, info) {
                        stream.on('data', function(chunk) {
                            count += chunk.length;
                            buffer += chunk.toString('utf8');
                        });
                        stream.once('end', function() {
                            if (info.which === 'TEXT')
                                console.log(prefix + 'Load body [%s] Finished', inspect(info.which));
                        });
                    });
                });
                f.once('error', function(err) {
                    console.log('Fetch error: ' + err);
                });
                f.once('end', function() {
                    console.log('Done fetching all messages!');
                    for (var i = arrContentKingPackage.length - 1; i >= 0; i--) {
                        if (buffer.indexOf(arrContentKingPackage[i]) > -1) {
                            assert.ok(true, arrContentKingPackage[i] + ' found');
                        } else {
                            assert.ok(false, arrContentKingPackage[i] + ' not found');
                        };
                    };
                    imap.end();
                });
            });
        });
    });
});

imap.once('error', function(err) {
    console.log('Fetch error: ' + err);
});

imap.once('end', function() {
    console.log('IMAP Connection ended');
});

imap.connect();