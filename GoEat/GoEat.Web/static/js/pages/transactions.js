function TransactionHistoryCtrl($scope) {
    $scope.months = [
       { "key": "1", "value": "January" },
       { "key": "2", "value": "February" },
       { "key": "3", "value": "March" },
       { "key": "4", "value": "April" },
       { "key": "5", "value": "May" },
       { "key": "6", "value": "June" },
       { "key": "7", "value": "July" },
       { "key": "8", "value": "August" },
       { "key": "9", "value": "September" },
       { "key": "10", "value": "October" },
       { "key": "11", "value": "November" },
       { "key": "12", "value": "December" },
    ]

    $scope.days = [];
    for (var i = 1; i <= 31; i++) {
        $scope.days.push(i);
    }
    
    $scope.years = [];
    for (var i = 2015; i <= 2020; i++) {
        $scope.years.push(i);
    }
    
}
gdineApp.controller('TransactionHistoryCtrl', ['$scope', TransactionHistoryCtrl]);

function historyTransaction(isFilter, isPrevious) {
    var index = $('input[id="pageIndex"]').val();
    if (!isFilter) {
        if (isPrevious) {
            index--;
        }
        else {
            index++;
        }
    }

    var fDay = $('select[id="fDay"]:visible').val();
    var fMonth = $('select[id="fMonth"]:visible').val();
    var fYear = $('select[id="fYear"]:visible').val();
    var tDay = $('select[id="tDay"]:visible').val();
    var tMonth = $('select[id="tMonth"]:visible').val();
    var tYear = $('select[id="tYear"]:visible').val();

    var fromDate = fYear + '-' + (fMonth.length > 1 ? fMonth : '0' + fMonth) + '-' + (fDay.length > 1 ? fDay : '0' + fDay);
    var toDate = tYear + '-' + (tMonth.length > 1 ? tMonth : '0' + tMonth) + '-' + (tDay.length > 1 ? tDay : '0' + tDay);
    if (Date.parse(fromDate) > Date.parse(toDate)) {
        alert('From date can not larger than To Date. Please re-arrange the filter.');
        return false;
    }
    else {
        $.post("/transaction/get-transaction",
            {
                page: index,
                fromDay: fDay,
                fromMonth: fMonth,
                fromYear: fYear,
                toDay: tDay,
                toMonth: tMonth,
                toYear: tYear,
            },
            function (data) {
                $("#divTransactionHistory").html(data);
            });
    }
};

$(document).ready(function () {
    
    var fDate = new Date();
    var month = fDate.getMonth() + 1;
    var day = fDate.getDate();
    var year = fDate.getFullYear();
    $('select[id="fDay"]:visible').val(day);
    $('select[id="fMonth"]:visible').val(month);
    $('select[id="fYear"]:visible').val(year);
    $('select[id="tDay"]:visible').val(day);
    $('select[id="tMonth"]:visible').val(month);
    $('select[id="tYear"]:visible').val(year);

    $("#divTransactionHistory").load("/transaction/get-transaction", {
        page: 1,
        fromDay: day,
        fromMonth: month,
        fromYear: year,
        toDay: day,
        toMonth: month,
        toYear: year,
    });
})