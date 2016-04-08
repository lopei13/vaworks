'use strict';
app.controller('searchController',
    ['$scope', '$http',
    function ($scope, $http) {

        $scope.loading = true;

        $scope.organizationId;
        $scope.valves;
       
        $scope.actuators;

        $scope.kit;
        $scope.selectedValve;   
        $scope.selectedActuator;
        $scope.hasValves = false;
        $scope.hasActuators = false;
        $scope.hasKit = false;
        $scope.currentStep = 1;
        $scope.searchText = "VA4565";
        $scope.kits;
        $scope.hasSearchResults = true;

        $scope.page = function (page) {
            if (page == 3) {
                if (!$scope.hasKit) {
                    page -= 1;
                }
            }

            $scope.currentStep = page;
        };

        $scope.getValves = function () {

            $http({
                method: "GET",
                url: "/Kits/GetValves",
                params: { kitId: $scope.kit.KitId, organizationId: $scope.organizationId }
            }).then(function success(res) {
                $scope.valves = res.data;
                $scope.hasValves = $scope.valves.length > 0;
                $scope.$emit('DONE');
            }, function error(res) {
                alert('No valves found.');
            }).finally(function () {
                $scope.loading = false;
            });
        };

        $scope.getActuators = function () {

            $http({
                method: "GET",
                url: "/Kits/GetActuators",
                params: { kitId: $scope.kit.KitId, organizationId: $scope.organizationId }
            }).then(function success(res) {
                $scope.actuators = res.data;
                $scope.hasActuators = $scope.actuators.length > 0;
                $scope.$emit('DONE');
            }, function error(res) {
                alert('No actuators found.');
            }).finally(function () {
                $scope.loading = false;
            });
        };

        $scope.keyDownSearch = function ($event) {
            if ($event.keyCode === 13) {
                $scope.search();
            }
        }

        $scope.search = function () {
            $scope.loading = true;
            $scope.kits = [];
            $http({
                method: "GET",
                url: "/Kits/GetKits",
                params: { searchText: $scope.searchText, organizationId: $scope.organizationId }
            }).then(function success(res) {
                $scope.kits = res.data;
                $scope.hasSearchResults = $scope.kits.length > 0;
                $scope.$emit('DONE');
            }, function error(res) {
                $scope.hasSearchResults = false;
            }).finally(function () {
                $scope.loading = false;
            });
        }

        $scope.selectKit = function (id) {
            for (var i = 0; i < $scope.kits.length; i++){
                if ($scope.kits[i].KitId === id) {
                    $scope.kit = $scope.kits[i];
                    $scope.hasKit = true;
                    $scope.page(2);
                    $scope.getValves();
                    $scope.getActuators();
                }
            }
        }

        $scope.selectValve = function (valve) {
            $scope.selectedValve = valve;
        }

        $scope.selectActuator = function (act) {
            $scope.selectedActuator = act;
        }

    }]);