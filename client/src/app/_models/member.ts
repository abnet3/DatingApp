import {Photo} from './photos';  
  
  export interface Member {
        id: number;
        username: string;
        age: number;
        photoUrl: string;
        knownAs: string;
        created: Date;
        lastActive: Date;
        gender: string;
        introduction: string;
        lookingFor: string;
        interests: string;
        city: string;
        country: string;
        photos: Photo[];
    }

   




