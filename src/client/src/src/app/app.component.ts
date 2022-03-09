import { Component, OnInit }   from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';

@Component( { selector    : 'app-root'
            , templateUrl : './app.component.html'
            , styleUrls   : [ './app.component.scss' ] } )
export class AppComponent implements OnInit {
    title = 'Prometheus';

    constructor( public readonly oidcSecurityService : OidcSecurityService ) { }

    ngOnInit(): void {
        this.oidcSecurityService.checkAuth().subscribe( auth => {
            console.log( 'CheckAuth' );
            console.table( auth );
        } );
    }

    public login = () : void =>
        void this.oidcSecurityService.authorize();
    
    public logout = () : void =>
        void this.oidcSecurityService.logoff();
}
