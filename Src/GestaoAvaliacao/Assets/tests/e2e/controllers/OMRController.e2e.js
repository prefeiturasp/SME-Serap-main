describe('Rota ->> File/IndexOMR', function () {

    beforeEach(function () {
        
    });

    it('deve realizar login', function () {

        browser.get("http://localhost:54296/");

        var el_login = element(by.id("ctl00_ContentPlaceHolder1__txtLogin"));
        el_login.click();
        el_login.sendKeys("admin");

        var el_password = element(by.id("ctl00_ContentPlaceHolder1__txtSenha"));
        el_password.click();
        el_password.sendKeys("123456");

        var el_button = element(by.id("ctl00_ContentPlaceHolder1__btnEntrar"));
        el_button.click();

        expect('certo').toEqual('certo');
    });
});