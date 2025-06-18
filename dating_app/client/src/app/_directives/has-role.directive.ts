import { Directive, inject, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';


import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRole]',
  standalone: true
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole:string[] = []; //parent to child
  private accountservice = inject(AccountService);
  private ViewContainerRef = inject(ViewContainerRef);
  private templateRef = inject(TemplateRef);

  ngOnInit(): void {
    if (this.accountservice.roles()?.some((r:string) =>this.appHasRole.includes(r)))
    {
      this.ViewContainerRef.createEmbeddedView(this.templateRef)
    }
    else
    {
      this.ViewContainerRef.clear()
    }
  }
}
