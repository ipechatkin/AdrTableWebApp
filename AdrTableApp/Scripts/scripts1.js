
var tableApp = angular.module('table-app', ['ngBootstrap', 'ngResource', 'ui.bootstrap'])

tableApp.service('translationService', translationService);
tableApp.controller('FrontController', FrontController);

function translationService($resource) {
    this.getTranslation = function ($scope, language) {
        var languageFilePath = '/Content/lang/lng_' + language + '.txt';
        $resource(languageFilePath).get(function (data) {
            $scope.translation = data;
        });
    };
};

function FrontController($http, translationService) {
    var vm = this;

    //array of records for the table
    vm.records = [];
    //all records count in the database
    vm.allRecordsCnt = 0;
    //Math injecting
    vm.Math = window.Math;
    
    vm.selectedLanguage = 'ru';

    vm.pageSize = 100;
    vm.numPages = 0;


    //order by specified column
    vm.predicate = -1;
    //printing order - asc or desc

    vm.reverse = false;
    vm.needSort = false;

    vm.currentPage = 1;

    vm.maxVisibleButtons = 10;

    vm.search = {};

    //daterangepicker init
    var dateRangeSt = NaN;	//start point of range for filter
    var dateRangeEnd = NaN; //end point of range for filter
    var dateStartToServer = '';
    var dateEndToServer = '';

    var dateRangeElem = angular.element(document.querySelector('#input-dateRange'));
    var dateRangeOptions = {
        opens: "left",
        timePicker: true,
        timePickerIncrement: 1,
        timePicker24Hour: true,
        cancelClass: "invisible",
        applyClass: "btn btn-primary glyphicon glyphicon-ok",
        locale: {
            format: 'DD/MM/YYYY HH:mm',
            applyLabel: ''
        }
    }

    function dateRangeUpdatedCallback(start, end, label) {
        alert("yyy");
        dateRangeSt = start;
        dateStartToServer = dateRangeSt.format('YYYY-MM-DD HH:mm');
        dateRangeEnd = end;
        dateEndToServer = dateRangeEnd.format('YYYY-MM-DD HH:mm');
        vm.currentPage = 1;
        //vm.pageChanged();
    }

    //If daterangepicker is closed without date choosing and input is empty, 
    //the input textbox contains a value of a date, but the model 'this.dateRange' doesn't.
    //So, I need to handle an event when picker is closing and check if a date is correctly choosen or not.
    //If it is not - erase this.
    function ifDateSelectCancelled_WORKAROUND() {
        dateRangeElem.on('hide.daterangepicker', function (ev, picker) {
            var inputText = document.getElementById("input-dateRange").value;
            alert("xxx");
            if (isNaN(dateRangeSt) && isNaN(dateRangeEnd) && inputText) {
                document.getElementById("input-dateRange").value = '';
                document.getElementById("resetDateFilter").click();
            }
        });
    }

    vm.translate = function () {
        var wasClear = ((document.getElementById("input-dateRange").value == '') ? true : false);
        translationService.getTranslation(vm, vm.selectedLanguage);
        //change locale and reconfigure the dateRangePicker
        moment.locale(vm.selectedLanguage);
        dateRangeElem.daterangepicker(dateRangeOptions, dateRangeUpdatedCallback);
        if (wasClear) {
            vm.dateRange = '';
            dateRangeSt = NaN;
            dateRangeEnd = NaN;
        }
        ifDateSelectCancelled_WORKAROUND();
    };

    vm.translate();

    vm.changeLang = function (language) {
        if (vm.selectedLanguage != language) {
            vm.selectedLanguage = (('en' == language) ? language : 'ru');
            vm.translate();
        }
    }

    vm.order = function (predicate) {
        vm.reverse = (vm.predicate === predicate) ? !vm.reverse : false;
        vm.predicate = predicate;
        vm.needSort = true;
        vm.pageChanged();
    }

    vm.dateRangeSearch = function (record) {
        if (isNaN(dateRangeSt) && isNaN(dateRangeEnd)) {
            return true;
        } else {
            var dateVal = moment(record.created, 'YYYY/MM/DD HH:mm:ss').valueOf();
            if ((dateVal >= dateRangeSt) && (dateVal <= dateRangeEnd))
                return true;
            else
                return false;
        }
    }

    vm.dateRangeClear = function () {
        vm.dateRange = '';
        //dateRangeSt = NaN;
        //dateRangeEnd = NaN;
        dateStartToServer = '';
        dateEndToServer = '';
        vm.currentPage = 1;
        vm.pageChanged();
    }

    

    vm.resetFilter = function () {
        vm.predicate = -1;
        vm.reverse = false;
        vm.needSort = false;
        vm.search = {};
        vm.dateRangeClear();
    }

    vm.dateCountryClear = function () {
        vm.search.country = '';
        vm.currentPage = 1;
        vm.pageChanged();
    }

    vm.dateCityClear = function () {
        vm.search.city = '';
        vm.currentPage = 1;
        vm.pageChanged();
    }

    vm.dateStreetClear = function () {
        vm.search.street = '';
        vm.currentPage = 1;
        vm.pageChanged();
    }

    vm.dateHouseClear = function () {
        vm.search.house = '';
        vm.currentPage = 1;
        vm.pageChanged();
    }

    vm.datePostCodeClear = function () {
        vm.search.postcode = '';
        vm.currentPage = 1;
        vm.pageChanged();
    }

    //data loading:

    //using toastr instead of standart js alert
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-top-center",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": 0,
        "extendedTimeOut": 0,
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut",
        "tapToDismiss": false
    }


    var pageData = {
            from: 0,
            count: vm.pageSize,
            country: '',
            city: '',
            street: '',
            dateStart: '',
            dateEnd: '',
            needSort: false,
            sortDir: false,
            sortCol: 0
    };

    vm.sendPageData = function () {

        pageData.country = vm.search.country;
        pageData.city = vm.search.city;
        pageData.street = vm.search.street;
        pageData.postcode = vm.search.postcode;
        pageData.dateStart = dateStartToServer;
        pageData.dateEnd = dateEndToServer;
        pageData.needSort = vm.needSort;
        pageData.sortDir = vm.reverse;
        pageData.sortCol = vm.predicate;

        
        $http.post('/Main/GetData', pageData, { async: false }).success(function (data) {

            vm.records = data.adresses;
            
            vm.allRecordsCnt = data.recTotal;

            vm.numPages = Math.ceil(vm.allRecordsCnt / vm.pageSize + 0.5);


			//add an index to the model for showing in the table
		    for (var i = 0; i < vm.records.length; i++) {
		        var date = new Date(parseInt(vm.records[i].created.substr(6)));
		        vm.records[i].created = date;
			    //vm.records[i].index = vm.startIndex + i;
			}					
		}).error(function (data, status, header, config) {
		    toastr['error'](vm.translation.srvAnswErr);
		});
	}
	vm.sendPageData();

	vm.pageChanged = function () {
	    pageData.from = (vm.currentPage - 1) * vm.pageSize;
	    vm.sendPageData();
	}


	vm.dateFilterGo = function () {
	    vm.sendPageData();
	}
}
