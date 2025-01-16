import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MembersListComponent } from './members/members-list/members-list.component';
import { MemberDetailsComponent } from './members/member-details/member-details.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { authGuard } from './_guards/auth.guard';
import { TestErrorsComponent } from './errors/test-errors/test-errors.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { preveventUnsavedChangesGuard } from './_guards/prevevent-unsaved-changes.guard';

export const routes: Routes = [
    {path:'',component:HomeComponent},
    {path:'',
        runGuardsAndResolvers:'always',canActivate:[authGuard],children:[
            {path:'members',component:MembersListComponent},
            {path:'members/:username',component:MemberDetailsComponent},
            {path:'member/edit',component:MemberEditComponent, 
                canDeactivate:[preveventUnsavedChangesGuard]},
            {path:'lists',component:ListsComponent},
            {path:'messages',component:MessagesComponent},
        ]
    },
    {path:'errors',component:TestErrorsComponent},
    {path:'not-found',component:NotFoundComponent},
    {path:'server-error',component:ServerErrorComponent},
    {path:'**',component:HomeComponent,pathMatch:'full'},
];
