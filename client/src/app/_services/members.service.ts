import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';


// const httpOptions = {

//   headers: new HttpHeaders({
//     Authorization: 'Bearer ' + JSON.parse(localStorage.getItem('user') || '{}')?.token
//   })
// }
@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environment.apiUrl;

  members: Member[] = [];

  // paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();

  memberCache = new Map();

  userParams!: UserParams;
  user!: User;
  // constructor(private http: HttpClient) {

  // }

  //after caching
 constructor(private http: HttpClient,  private accountService: AccountService) {

  this.accountService.currentUser$.pipe(take(1)).subscribe((user)=> {

    this.user = user;
    this.userParams = new UserParams(user);
  })
}
  getUserParams(){
  
    return this.userParams;
  }
  setUserParams(params: UserParams){
  
    this.userParams = params;
  }

  resetUserParams(){

    this.userParams = new UserParams(this.user);
    return this.userParams;
  }

  // getMembers(){


  //   if(this.members.length > 0) return of(this.members);
  //     return this.http.get<Member[]>(this.baseUrl + 'users').pipe(map(members => {

  //       this.members = members;
  //       return members;
  //     }))

  //   }
  // return this.http.get<Member[]>(this.baseUrl + 'users', httpOptions);
  // return this.http.get<Member[]>(this.baseUrl + 'users');

  //after pagination 
  // getMembers(page?: number, itemsPerPage?: number) {
  getMembers(userParams: UserParams) {

    // console.log(Object.values(userParams).join('-'));

          var respose = this.memberCache.get(Object.values(userParams).join('-'));
          
          if(respose){

            return of(respose);
          }



    // let params = new HttpParams();

    // if (page !== null && itemsPerPage !== null) {

    //   params = params.append('pageNumber', page!.toString());
    //   params = params.append('pageSize', itemsPerPage!.toString());

    // }

    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    // return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params)

        // return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params)

        // after paginaitonHelper

        return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http)

        .pipe(map(response=> {
          this.memberCache.set(Object.values(userParams).join('-'), response);
          return response;
        }))


  }


       //Moved to paginationHelpers to reuse this

  // private getPaginatedResult<T>(url: any, params: any) {
  //   //@ts-ignore
  //   const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
  //   return this.http.get<T>(url, { observe: 'response', params }).pipe(
  //     map(response => {
  //       //@ts-ignore
  //       paginatedResult.result = response.body;
  //       if (response.headers.get('Pagination') !== null) {
  //         paginatedResult.pagination = JSON.parse(response.headers.get('Pagination') || '{}');
  //       }
  //       return paginatedResult;
  //     })
  //   );
  // }

  // private getPaginationHeaders(pageNumber: number, pageSize: number) {

  //   let params = new HttpParams();

  //   params = params.append('pageNumber', pageNumber.toString());
  //   params = params.append('pageSize', pageSize.toString());

  //   return params;

  // }



  // getMember(username: string) {
  //   const member = this.members.find(x => x.username === username);
  //   if (member !== undefined) return of(member);


  //   return this.http.get<Member>(this.baseUrl + 'users/' + username)
  // }


  //after cache
  getMember(username: string) {
    // console.log(this.memberCache); 
    const member = [...this.memberCache.values()].reduce((arr, elem) => arr.concat(elem.result), [])
    .find((member: Member) => member.username === username);

    if(member){
      return of(member);
    }
    // console.log(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username)
  }


  updatemember(member: Member) {


    return this.http.put(this.baseUrl + 'users', member).pipe(map(

      () => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      }
    ))
  }

  setMainPhoto(photoId: number) {

    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {

    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }


  addLike(username: string){

     return this.http.post(this.baseUrl + 'likes/' + username, {});
  }

  //@ts-ignore
  // getLikes(predicate:string, pageNumber, pageSize){
  //   let params = this.getPaginationHeaders(pageNumber, pageSize)
  //   params = params.append('predicate', predicate)
  //   return this.getPaginatedResult<Partial<Member[]>>(this.baseUrl + 'likes', params);

  //   // return this.http.get<Partial<Member[]>>(this.baseUrl + 'likes?predicate=' + predicate);
  // }

  //agter paginationHelper global class

  getLikes(predicate:string, pageNumber, pageSize){
    let params = getPaginationHeaders(pageNumber, pageSize)
    params = params.append('predicate', predicate)
    return getPaginatedResult<Partial<Member[]>>(this.baseUrl + 'likes', params, this.http);

    // return this.http.get<Partial<Member[]>>(this.baseUrl + 'likes?predicate=' + predicate);
  }
}
