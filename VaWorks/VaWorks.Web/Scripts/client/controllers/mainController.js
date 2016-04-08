'use strict';
app.controller('mainController',
    ['$scope', '$http',
    function ($scope, $http) {

        $scope.loading = true;

        $scope.organizationId;
        $scope.quantity = 1;
        $scope.searchActive = false;
        $scope.searchToggleButtonText = "Search Kit"

        $scope.toggleSearch = function () {
            $scope.searchActive = !$scope.searchActive;
            if ($scope.searchActive) {
                $scope.searchToggleButtonText = "Select Kit";
            } else {
                $scope.searchToggleButtonText = "Search Kit";
            }
        }

    }]);