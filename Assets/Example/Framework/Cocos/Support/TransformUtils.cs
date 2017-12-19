using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Microsoft.Xna.Framework;
using UnityEngine;

namespace CocosFramework
{
    public class TransformUtils
    {
        public static Matrix4x4 CGAffineToMatrix(float[] m)
        {
            Matrix4x4 mat = new Matrix4x4()
            {
                m00 = m[0],m10 = m[4],m20 = m[8],m30 = m[12],
                m01 = m[1],m11 = m[5],m21 = m[9],m31 = m[13],
                m02 = m[2],m12 = m[6],m22 = m[10],m32 = m[14],
                m03 = m[3],m13 = m[7],m23 = m[11],m33 = m[15],
            };

            return mat;
        }

        public static Matrix4x4 CGAffineToMatrix(CCAffineTransform t)
        {
            float[] m = new float[16];
            CGAffineToGL(t, ref m);
            return CGAffineToMatrix(m);
        }

        public static void CGAffineToGL(CCAffineTransform t, ref float[] m)
        {
            // | m[0] m[4] m[8]  m[12] |     | m11 m21 m31 m41 |     | a c 0 tx |
            // | m[1] m[5] m[9]  m[13] |     | m12 m22 m32 m42 |     | b d 0 ty |
            // | m[2] m[6] m[10] m[14] | <=> | m13 m23 m33 m43 | <=> | 0 0 1  0 |
            // | m[3] m[7] m[11] m[15] |     | m14 m24 m34 m44 |     | 0 0 0  1 |

            m[2] = m[3] = m[6] = m[7] = m[8] = m[9] = m[11] = m[14] = 0.0f;
            m[10] = m[15] = 1.0f;
            m[0] = t.a; m[4] = t.c; m[12] = t.tx;
            m[1] = t.b; m[5] = t.d; m[13] = t.ty;
        }

        public static void GLToCGAffine(float[] m, CCAffineTransform t)
        {
            t.a = m[0]; t.c = m[4]; t.tx = m[12];
            t.b = m[1]; t.d = m[5]; t.ty = m[13];
        }
    }
}
