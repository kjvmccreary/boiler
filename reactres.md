PS C:\Users\mccre\dev\boiler\src\frontend\react-app> npm run build

> microservices-frontend@0.0.0 build
> tsc -b && vite build

src/services/user.service.ts:58:28 - error TS2339: Property 'items' does not exist on type '{ success: boolean; data: { items: User[]; totalCount: number; pageNumber: number; pageSize: number; totalPages: number; }; }'.

58       data: response.data?.items || [],
                              ~~~~~

src/services/user.service.ts:59:34 - error TS2339: Property 'totalCount' does not exist on type '{ success: boolean; data: { items: User[]; totalCount: number; pageNumber: number; pageSize: number; totalPages: number; }; }'.

59       totalCount: response.data?.totalCount || 0,
                                    ~~~~~~~~~~

src/services/user.service.ts:60:34 - error TS2339: Property 'pageNumber' does not exist on type '{ success: boolean; data: { items: User[]; totalCount: number; pageNumber: number; pageSize: number; totalPages: number; }; }'.

60       pageNumber: response.data?.pageNumber || 1,
                                    ~~~~~~~~~~

src/services/user.service.ts:61:32 - error TS2339: Property 'pageSize' does not exist on type '{ success: boolean; data: { items: User[]; totalCount: number; pageNumber: number; pageSize: number; totalPages: number; }; }'.

61       pageSize: response.data?.pageSize || 10,
                                  ~~~~~~~~

src/services/user.service.ts:62:34 - error TS2339: Property 'totalPages' does not exist on type '{ success: boolean; data: { items: User[]; totalCount: number; pageNumber: number; pageSize: number; totalPages: number; }; }'.

62       totalPages: response.data?.totalPages || 0
                                    ~~~~~~~~~~


Found 5 errors.
