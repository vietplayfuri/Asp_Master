var Imap = require('imap'),
    inspect = require('util').inspect,
    assert = require('assert');
var imapSupport = new Imap({
    user: 'hi@gtoken.com',
    password: 'GEEtoken614',
    host: 'imap.gmail.com',
    port: 993,
    tls: true
});

function openInbox(cb) {
    imapSupport.openBox('INBOX', false, cb);
}

var arrContentTC01 = ['No reply (testing from GToken)', 'website', 'It is a test for Website support', 'FoxyMax'];
var searchEmailTC01 = 'phuong.gtoken@gmail.com';

var arrContentTC02 = ['No reply (testing from GToken)', 'No Customer Name', 'Mine Mania', 'It is a test for Game support', 'ios - version: 8.01', 'Iphone 6+'];
var searchEmailTC02 = 'phuong.gtoken+supportgame@gmail.com';

var subject = 'No reply (testing from GToken)';

// Test sending for website support
imapSupport.once('ready', function() {
    var buffer = '',
        count = 0;
    openInbox(function(err, box) {
        if (err) throw err;
        imapSupport.search([
            ['HEADER', 'SUBJECT', searchEmailTC01]
        ], function(err, results) {
            if (err) throw err;
            var f = imapSupport.fetch(results, {
                bodies: ['TEXT']
            });
            f.on('message', function(msg, seqno) {
                console.log('Message #%d', seqno);
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
                console.log('Done testing website messages!');
                for (var i = arrContentTC01.length - 1; i >= 0; i--) {
                    if (buffer.indexOf(arrContentTC01[i]) > -1) {
                        assert.ok(true, arrContentTC01[i] + ' found');
                    } else {
                        assert.ok(false, arrContentTC01[i] + ' not found');
                    };
                };
                imapSupport.end();
            });
        });
    });
});

// Test sending for game support
imapSupport.once('ready', function() {
    var buffer = '',
        count = 0;
    openInbox(function(err, box) {
        if (err) throw err;
        imapSupport.search([
            ['HEADER', 'SUBJECT', searchEmailTC02]
        ], function(err, results) {
            if (err) throw err;
            var f = imapSupport.fetch(results, {
                bodies: ['TEXT']
            });
            f.on('message', function(msg, seqno) {
                console.log('Message #%d', seqno);
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
                console.log('Done testing Game messages!');
                for (var i = arrContentTC02.length - 1; i >= 0; i--) {
                    if (buffer.indexOf(arrContentTC02[i]) > -1) {
                        assert.ok(true, arrContentTC02[i] + ' found');
                    } else {
                        assert.ok(false, arrContentTC02[i] + ' not found');
                    };
                };
                imapSupport.end();
            });
        });
    });
});

//Delete Email by subject
imapSupport.once('ready', function() {
    openInbox(function(err, box) {
        if (err) throw err;
        imapSupport.search([
            ['SUBJECT', subject]
        ], function(err, results) {
            if (results.length > 0) {
                imapSupport.addFlags(results, 'Deleted', function(err) {
                    assert.ok(true, 'The email by subject has been deleted');
                    console.log('Delete all emails sent from phuong.gtoken@gmail.com');
                });
            };
            imapSupport.end();
        });
    });
});

imapSupport.once('error', function(err) {
    console.log('Fetch error: ' + err);
});

imapSupport.once('end', function() {
    console.log('imapSupport Connection ended');
});

imapSupport.connect();
