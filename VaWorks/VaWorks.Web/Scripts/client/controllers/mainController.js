'use strict';
app.controller('mainController',
    ['$scope', '$http',
    function ($scope, $http) {

        $scope.loading = true;

        $scope.valves;
        $scope.valvemodels;
        $scope.valvesizes;
        $scope.actuators;
        $scope.actuatormodels;
        $scope.actuatorsizes;
        $scope.materials;
        $scope.options;
        $scope.kit;
        $scope.selectedValve;
        $scope.selectedValveModel;
        $scope.selectedValveSize;
        $scope.selectedActuator;
        $scope.selectedActuatorModel;
        $scope.selectedActuatorSize;
        $scope.selectedMaterial;
        $scope.hasValves = false;
        $scope.hasValveModels = false;
        $scope.hasValveSizes = false;
        $scope.hasActuators = false;
        $scope.hasActuatorModels = false;
        $scope.hasActuatorSizes = false;
        $scope.hasMaterials = false;
        $scope.hasOptions = false;
        $scope.hasKit = false;
        $scope.currentStep = 0;
        $scope.quantity = 1;

        $scope.page = function (page) {
            $scope.currentStep = page;
        };

        $http({
            method: "GET",
            url: "/KitSelection/GetValveManufacturers"
        }).then(function success(res){
            $scope.valves = res.data;
            $scope.hasValves = $scope.valves.length > 0;
            $scope.page(1);
            $scope.$emit('DONE');
        }, function error(res){
            alert('No valves found.');
        }).finally(function () {
            $scope.loading = false;
        });

        $scope.getValveModels = function () {
            $scope.loading = true;
            $http({
                method: "GET",
                url: "/KitSelection/GetValveModels",
                params: { mfg: $scope.selectedValve }
            }).then(function success(res) {
                $scope.valvemodels = res.data;
                $scope.hasValveModels = $scope.valvemodels.length > 0;
            }, function error(res) {
                alert('No valves found.');
            }).finally(function () {
                $scope.loading = false;
            });
        };

        $scope.getValveSizes = function () {
            $scope.loading = true;
            $http({
                method: "GET",
                url: "/KitSelection/GetValveSizes",
                params: { mfg: $scope.selectedValve, model: $scope.selectedValveModel }
            }).then(function success(res) {
                $scope.valvesizes = res.data;
                $scope.hasValveSizes = $scope.valvesizes.length > 0;
            }, function error(res) {
                alert('No valves found.');
            }).finally(function () {
                $scope.loading = false;
            });
        };

        $scope.getActuators = function () {
            $scope.loading = true;
            $http({
                method: "GET",
                url: "/KitSelection/GetActuators",
                params: { valveInterface: $scope.selectedValveSize.InterfaceCode }
            }).then(function success(res) {
                $scope.actuators = res.data;
                $scope.hasActuators = $scope.actuators.length > 0;
            }, function error(res) {
                alert('No actuators found.');
            }).finally(function () {
                $scope.loading = false;
            });
        };

        $scope.getActuatorModels = function () {
            $scope.loading = true;
            $http({
                method: "GET",
                url: "/KitSelection/GetActuatorModels",
                params: { valveInterface: $scope.selectedValveSize.InterfaceCode, mfg: $scope.selectedActuator }
            }).then(function success(res) {
                $scope.actuatormodels = res.data;
                $scope.hasActuatorModels = $scope.actuatormodels.length > 0;
            }, function error(res) {
                alert('No actuators found.');
            }).finally(function () {
                $scope.loading = false;
            });
        };

        $scope.getActuatorSizes = function () {
            $scope.loading = true;
            $http({
                method: "GET",
                url: "/KitSelection/GetActuatorSizes",
                params: { valveInterface: $scope.selectedValveSize.InterfaceCode, mfg: $scope.selectedActuator, model: $scope.selectedActuatorModel }
            }).then(function success(res) {
                $scope.actuatorsizes = res.data;
                $scope.hasActuatorSizes = $scope.actuatorsizes.length > 0;
            }, function error(res) {
                alert('No actuators found.');
            }).finally(function () {
                $scope.loading = false;
            });
        };

        $scope.getMaterials = function () {
            $scope.loading = true;
            $http({
                method: "GET",
                url: "/KitSelection/GetKitMaterials",
                params: { valveInterface: $scope.selectedValveSize.InterfaceCode, actuatorInterface: $scope.selectedActuatorSize.InterfaceCode }
            }).then(function success(res) {
                $scope.materials = res.data;
                $scope.hasMaterials = $scope.materials.length > 0;
                $scope.page(2);
            }, function error(res) {
                alert('No materials found.');
            }).finally(function () {
                $scope.loading = false;
            });
        };


        $scope.getOptions = function () {
            $scope.loading = true;
            $http({
                method: "GET",
                url: "/KitSelection/GetKitOptions",
                params: { valveInterface: $scope.selectedValveSize.InterfaceCode, actuatorInterface: $scope.selectedActuatorSize.InterfaceCode, materialId: $scope.selectedMaterial.KitMaterialId }
            }).then(function success(res) {
                $scope.options = res.data;
                $scope.hasOptions = $scope.options.length > 0;
                $scope.page(3);
            }, function error(res) {
                alert('No options found.');
            }).finally(function () {
                $scope.loading = false;
            });
        };

        $scope.getKit = function () {
            $scope.loading = true;
            $http({
                method: "GET",
                url: "/KitSelection/GetKit",
                params: { valveInterface: $scope.selectedValveSize.InterfaceCode, actuatorInterface: $scope.selectedActuatorSize.InterfaceCode, materialId: $scope.selectedMaterial.KitMaterialId, optionId: $scope.selectedOption.KitOptionId }
            }).then(function success(res) {
                if (res.data.length > 0) {
                    $scope.kit = res.data[0];
                    $scope.hasKit = true;
                    $scope.page(4);
                }
            }, function error(res) {
                alert('No kit found.');
            }).finally(function () {
                $scope.loading = false;
            });
        };

    }]);