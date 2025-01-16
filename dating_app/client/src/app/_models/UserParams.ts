import { User } from "./User";

export class UserParams {
    gender:string;
    minAge=18;
    maxAge=99;
    PageNumber =1 ;
    PageSize = 5 ;
    orderBy="lastActive"
    constructor(user : User | null) {
        this.gender = user?.gender === 'female' ? 'male' :'female'
    }
}