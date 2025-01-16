import { Photo } from "./Photo"

export interface Member {
    id: number
    username: string
    age: number
    photoUrl: string
    knownAS: string
    created: Date
    lastActive: Date
    gender: string
    introdutrion: any
    interests: string
    lookingFor: string
    city: string
    country: string
    photos: Photo[]
  }
  
