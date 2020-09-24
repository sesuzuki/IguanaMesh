using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IGmshWrappers
{
    /// <summary>
    /// Elements Type (number reference);
    /// (2) => 3-node triangle.
    /// (3) => 4-node quadrangle.
    /// 4 => 4-node tetrahedron. 
    /// 5 => 8-node hexahedron. 
    /// 6 => 6-node prism. 
    /// 7 => 5-node pyramid.
    /// 8 => 3-node second order line (2 nodes associated with the vertices and 1 with the edge).
    /// (9) = 6-node second order triangle (3 nodes associated with the vertices and 3 with the edges).
    /// (10) => 9-node second order quadrangle (4 nodes associated with the vertices, 4 with the edges and 1 with the face).
    /// 11 => 10-node second order tetrahedron(4 nodes associated with the vertices and 6 with the edges). 
    /// 12 => 27-node second order hexahedron(8 nodes associated with the vertices, 12 with the edges, 6 with the faces and 1 with the volume). 
    /// 13 => 18-node second order prism(6 nodes associated with the vertices, 9 with the edges and 3 with the quadrangular faces). 
    /// 14 => 14-node second order pyramid(5 nodes associated with the vertices, 8 with the edges and 1 with the quadrangular face). 
    /// 15 >= 1-node point.
    /// (16) => 8-node second order quadrangle(4 nodes associated with the vertices and 4 with the edges). 
    /// 17 => 20-node second order hexahedron(8 nodes associated with the vertices and 12 with the edges). 
    /// 18 => 15-node second order prism(6 nodes associated with the vertices and 9 with the edges). 
    /// 19 => 13-node second order pyramid(5 nodes associated with the vertices and 8 with the edges). 
    /// (20) => 9-node third order incomplete triangle (3 nodes associated with the vertices, 6 with the edges)
    /// (21) => 10-node third order triangle(3 nodes associated with the vertices, 6 with the edges, 1 with the face)
    /// (22) => 12-node fourth order incomplete triangle(3 nodes associated with the vertices, 9 with the edges)
    /// (23) => 15-node fourth order triangle(3 nodes associated with the vertices, 9 with the edges, 3 with the face)
    /// 24 => 15-node fifth order incomplete triangle(3 nodes associated with the vertices, 12 with the edges)
    /// 25 => 21-node fifth order complete triangle(3 nodes associated with the vertices, 12 with the edges, 6 with the face)
    /// 26 => 4-node third order edge(2 nodes associated with the vertices, 2 internal to the edge) 
    /// 27 => 5-node fourth order edge(2 nodes associated with the vertices, 3 internal to the edge) 
    /// 28 => 6-node fifth order edge(2 nodes associated with the vertices, 4 internal to the edge) 
    /// 29 => 20-node third order tetrahedron(4 nodes associated with the vertices, 12 with the edges, 4 with the faces)
    /// 30 => 35-node fourth order tetrahedron(4 nodes associated with the vertices, 18 with the edges, 12 with the faces, 1 in the volume)
    /// 31 => 56-node fifth order tetrahedron(4 nodes associated with the vertices, 24 with the edges, 24 with the faces, 4 in the volume)
    /// 92 => 64-node third order hexahedron(8 nodes associated with the vertices, 24 with the edges, 24 with the faces, 8 in the volume)
    /// 93 => 125-node fourth order hexahedron(8 nodes associated with the vertices, 36 with the edges, 54 with the faces, 27 in the volume)
    /// </summary>
    class IguanaGmshElementType
    {
        ////

        /*
        Triangle:               Triangle6:          Triangle9/10:          Triangle12/15:

        v
        ^                                                                   2
        |                                                                   | \
        2                       2                    2                      9   8
        |`\                     |`\                  | \                    |     \
        |  `\                   |  `\                7   6                 10 (14)  7
        |    `\                 5    `4              |     \                |         \
        |      `\               |      `\            8  (9)  5             11 (12) (13) 6
        |        `\             |        `\          |         \            |             \
        0----------1 --> u      0-----3----1         0---3---4---1          0---3---4---5---1
            


        Quadrangle:            Quadrangle8:            Quadrangle9:

              v
              ^
              |
        3-----------2          3-----6-----2           3-----6-----2
        |     |     |          |           |           |           |
        |     |     |          |           |           |           |
        |     +---- | --> u    7           5           7     8     5
        |           |          |           |           |           |
        |           |          |           |           |           |
        0-----------1          0-----4-----1           0-----4-----1

        Tetrahedron:                          Tetrahedron10:

                           v
                         .
                       ,/
                      /
                   2                                     2
                 ,/|`\                                 ,/|`\
               ,/  |  `\                             ,/  |  `\
             ,/    '.   `\                         ,6    '.   `5
           ,/       |     `\                     ,/       8     `\
         ,/         |       `\                 ,/         |       `\
        0-----------'.--------1 --> u         0--------4--'.--------1
         `\.         |      ,/                 `\.         |      ,/
            `\.      |    ,/                      `\.      |    ,9
               `\.   '. ,/                           `7.   '. ,/
                  `\. |/                                `\. |/
                     `3                                    `3
                        `\.
                           ` w

        Hexahedron:             Hexahedron20:          Hexahedron27:

               v
        3----------2            3----13----2           3----13----2
        |\     ^   |\           |\         |\          |\         |\
        | \    |   | \          | 15       | 14        |15    24  | 14
        |  \   |   |  \         9  \       11 \        9  \ 20    11 \
        |   7------+---6        |   7----19+---6       |   7----19+---6
        |   |  +-- |-- | -> u   |   |      |   |       |22 |  26  | 23|
        0---+---\--1   |        0---+-8----1   |       0---+-8----1   |
         \  |    \  \  |         \  17      \  18       \ 17    25 \  18
          \ |     \  \ |         10 |        12|        10 |  21    12|
           \|      w  \|           \|         \|          \|         \|
            4----------5            4----16----5           4----16----5

                Prism:                      Prism15:               Prism18:

                   w
                   ^
                   |
                   3                       3                      3
                 ,/|`\                   ,/|`\                  ,/|`\
               ,/  |  `\               12  |  13              12  |  13
             ,/    |    `\           ,/    |    `\          ,/    |    `\
            4------+------5         4------14-----5        4------14-----5
            |      |      |         |      8      |        |      8      |
            |    ,/|`\    |         |      |      |        |    ,/|`\    |
            |  ,/  |  `\  |         |      |      |        |  15  |  16  |
            |,/    |    `\|         |      |      |        |,/    |    `\|
           ,|      |      |\        10     |      11       10-----17-----11
         ,/ |      0      | `\      |      0      |        |      0      |
        u   |    ,/ `\    |    v    |    ,/ `\    |        |    ,/ `\    |
            |  ,/     `\  |         |  ,6     `7  |        |  ,6     `7  |
            |,/         `\|         |,/         `\|        |,/         `\|
            1-------------2         1------9------2        1------9------2
         
     
        
                    Pyramid:                     Pyramid13:                   Pyramid14:

                           4                            4                            4
                         ,/|\                         ,/|\                         ,/|\
                       ,/ .'|\                      ,/ .'|\                      ,/ .'|\
                     ,/   | | \                   ,/   | | \                   ,/   | | \
                   ,/    .' | `.                ,/    .' | `.                ,/    .' | `.
                 ,/      |  '.  \             ,7      |  12  \             ,7      |  12  \
               ,/       .' w |   \          ,/       .'   |   \          ,/       .'   |   \
             ,/         |  ^ |    \       ,/         9    |    11      ,/         9    |    11
            0----------.'--|-3    `.     0--------6-.'----3    `.     0--------6-.'----3    `.
             `\        |   |  `\    \      `\        |      `\    \     `\        |      `\    \
               `\     .'   +----`\ - \ -> v  `5     .'        10   \      `5     .' 13     10   \
                 `\   |    `\     `\  \        `\   |           `\  \       `\   |           `\  \
                   `\.'      `\     `\`          `\.'             `\`         `\.'             `\`
                      1----------------2            1--------8-------2           1--------8-------2
                                `\
                                   u

         */
    }
}
