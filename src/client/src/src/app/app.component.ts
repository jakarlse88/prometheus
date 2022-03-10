import { Component, OnInit } from '@angular/core';
import { AuthService }        from '@auth0/auth0-angular';

@Component( { selector    : 'app-root'
            , templateUrl : './app.component.html'
            , styleUrls   : [ './app.component.scss' ] } )
export class AppComponent implements OnInit {
    title = 'Prometheus'

    constructor( public readonly auth : AuthService ) { }

    ngOnInit(): void {
    }

    public login = () : void =>
        void this.auth.loginWithRedirect()
    
    public logout = () : void =>
        void this.auth.logout( { returnTo: document.location.origin } )

}
