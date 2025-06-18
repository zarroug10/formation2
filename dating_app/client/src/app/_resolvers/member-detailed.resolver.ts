import { ResolveFn } from '@angular/router';
import { inject } from '@angular/core';


import { Member } from '../_models/Member';
import { MembersService } from '../_services/members.service';

export const memberDetailedResolver: ResolveFn<Member | null> = (route, state) => {
  const memberservice = inject(MembersService)

  const username = route.paramMap.get('username');

  if(!username) return null ;


  return memberservice.getMember(username);
};
