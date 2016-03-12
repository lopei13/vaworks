

var app = angular.module('MainApp', [
    'ngResource',
    'cgBusy']);

var url = 'http://localhost:50630/';
//var url = 'http://snlfiddle.azurewebsites.net/';

app.constant('apiUrl', {
    apiUrl: url
});


//app.run(['$rootScope', 'dataService', function ($rootScope, dataService) {
//    $rootScope.dataService = dataService;
//}]);