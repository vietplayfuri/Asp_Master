var Imap = require('imap'),
    inspect = require('util').inspect,
    assert = require('assert');

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

//      Delete All Email on phuong.gtoken@gmail.com
imap.once('ready', function() {
    openInbox(function(err, box) {
        if (err) throw err;
        var totalOfEmail = box.messages.total;
        imap.addFlags(box.messages.total + ':*', 'Deleted', function(err) {
            if (totalOfEmail > 0) {
                console.log('ALL EMAIL BE DELETED');
                assert.ok(true, 'ALL EMAIL BE DELETED');  
            } else {
                console.log('NO EMAIL TO DELETE');   
            };        
        });
        imap.end();
    });
});

imap.once('error', function(err) {
    console.log('Fetch error: ' + err);
});
imap.once('end', function() {
    console.log('IMAP Connection ended');
});
imap.connect();