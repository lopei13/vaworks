'use strict';
app.controller('selectController',
    ['$scope', '$http',
    function ($scope, $http) {

        $scope.loading = true;

        $scope.kitId;
        $scope.organizationId;
        $scope.valves;
        $scope.actuators;
        $scope.valveId;
        $scope.actuatorId;

        $http({
            method: "GET",
            url: "/Kits/KitValves",
            params: { kitId: $scope.kitId, organizationId: $scope.organizationId }
        }).then(function success(res){
            $scope.valves = res.data;
            $scope.$emit('DONE');
        }, function error(res){
            alert('No valves found.');
        }).finally(function () {
            $scope.loading = false;
        });

        $http({
            method: "GET",
            url: "/Kits/KitActuators",
            params: { kitId: $scope.kitId, organizationId: $scope.organizationId }
        }).then(function success(res) {
            $scope.actuators = res.data;
            $scope.$emit('DONE');
        }, function error(res) {
            alert('No actuators found.');
        }).finally(function () {
            $scope.loading = false;
        });

    }]);