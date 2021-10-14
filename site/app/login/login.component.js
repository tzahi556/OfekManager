(function () {
    
    var app = angular.module('app');

    app.component('login', {
        templateUrl: 'app/login/login.template.html',
        controller: LoginController,
        bindings: {
            returnUrl: '<'
        }
    });

    function LoginController(authenticationService, $state) {
        this.login = _login;

        this.email = "tzahi556@gmail.com";
        this.password = "123";
        function _login() {

           
            authenticationService.login({ userName: this.email, password: this.password }).then(function (res) {
               
                location.href = './';
            },
            function (res) {
                alert(res.error_description);
            });
        }
    }

})();