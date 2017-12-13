namespace cocos2d
{
    public interface ICCRGBAProtocol
    {
        /// <summary>
        /// Gets or sets the color
        /// </summary>
        ccColor3B Color { get; set; }

        /// <summary>
        /// Gets or sets the Opacity
        /// @warning If the the texture has premultiplied alpha then, the R, G and B channels will be modifed.
        /// Values goes from 0 to 255, where 255 means fully opaque.
        /// </summary>
        byte Opacity { get; set; }

        /** sets the premultipliedAlphaOpacity property.
	     If set to NO then opacity will be applied as: glColor(R,G,B,opacity);
	     If set to YES then oapcity will be applied as: glColor(opacity, opacity, opacity, opacity );
	     Textures with premultiplied alpha will have this property by default on YES. Otherwise the default value is NO
	     @since v0.8
	     */

        bool IsOpacityModifyRGB { get; set; }

        /** returns whether or not the opacity will be applied using glColor(R,G,B,opacity) or glColor(opacity, opacity, opacity, opacity);
	     @since v0.8
	     */
    }

    /// <summary>
    /// You can specify the blending fuction.
    /// @since v0.99.0
    /// </summary>
    public interface ICCBlendProtocol
    {
        /// <summary>
        /// gets or sets the source blending function for the texture
        /// </summary>
        ccBlendFunc BlendFunc { get; set; }
    }

    /// <summary>
    /// CCNode objects that uses a Texture2D to render the images.
    /// </summary>
    /// <remarks>
    /// The texture can have a blending function.
    /// If the texture has alpha premultiplied the default blending function is:
    ///   src=GL_ONE dst= GL_ONE_MINUS_SRC_ALPHA
    /// else
    ///    src=GL_SRC_ALPHA dst= GL_ONE_MINUS_SRC_ALPHA
    /// But you can change the blending funtion at any time.
    /// @since v0.8.0
    /// </remarks>
    public interface ICCTextureProtocol : ICCBlendProtocol
    {
        /// <summary>
        /// gets or sets a new texture. it will be retained
        /// </summary>
        CCTexture2D Texture { get; set; }
    }

    public interface ICCLabelProtocol
    {
        // sets a new label using an string
        void setString(string label);

        // returns the string that is rendered
        string getString();
    }

    /// <summary>
    /// OpenGL projection protocol
    /// </summary>
    public interface ICCProjectionProtocol
    {
        /// <summary>
        /// Called by CCDirector when the porjection is updated, and "custom" projection is used
        /// @since v0.99.5
        /// </summary>
        void updateProjection();
    }
}
