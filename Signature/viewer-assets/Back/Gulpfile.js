/**
 * Gulpfile.js
 *
 * Copyright (c) 2015 Accusoft Corporation. All rights reserved.
 */

/* jshint node: true */

var gulp = require('gulp');

// loads all gulp plugins from our package.json file
var $ = require('gulp-load-plugins')();
var ns = require('node-stream');

// --------------------
// Build full-viewer icons
// --------------------
var SvgSourceBase = './icons/svg';
var SvgSource = SvgSourceBase + '/*.svg';
var SvgCssTemplate = './icons/template.less';

function generateIconTask(opts) {
    // flatten the options
    var size = opts.size;
    var color = opts.color;
    var filename = opts.filename;
    var backgroundUrl = opts.backgroundUrl;
    var includeCss = !!opts.includeCss;

    var svgfallbackSettings = {
        backgroundUrl: backgroundUrl,
        spriteWidth: size * 18,
        cssTemplate: SvgCssTemplate
    };

    var pngFilter = $.filter('*.png',{restore: true});
    var cssFilter = $.filter('*.css');

    return ns.pipeline.obj(
        gulp.src(SvgSource, {
            base: SvgSourceBase
        }),
        $.cheerio(function(jQuery, file) {
            // add a width and height
            var iconSize = size + 'px';
            jQuery('svg').attr('width', iconSize)
                .attr('height', iconSize)
                .attr('fill', color)
                // remove fill from all sub-elements
                .find('[fill]').removeAttr('fill');
        }),
        $.svgfallback(svgfallbackSettings),
        // exclude all CSS files, because we just want to write images
        pngFilter,
        $.rename({
            basename: filename
        }),
        gulp.dest('./img'),

        // if including CSS, revert the filter, get the CSS file, and write it to the output
        $.if(!!includeCss, pngFilter.restore),
        cssFilter,
        $.rename({
            basename: filename,
            extname: '.less'
        }),
        gulp.dest('./less/base')
    );
}

gulp.task('icons:1x', function() {
    return generateIconTask({
        size: 26,
        color: '#7D7F85',
        filename: 'icons',
        backgroundUrl: '../img/icons.png',
        includeCss: true
    });
});
gulp.task('icons:2x', function() {
    return generateIconTask({
        size: 52,
        color: '#7D7F85',
        filename: 'icons@2x',
        backgroundUrl: '../img/icons.png'
    });
});
gulp.task('icons:1x:white', function() {
    return generateIconTask({
        size: 26,
        color: '#ffffff',
        filename: 'icons.white',
        backgroundUrl: '../img/icons.png'
    });
});
gulp.task('icons:2x:white', function() {
    return generateIconTask({
        size: 52,
        color: '#ffffff',
        filename: 'icons.white@2x',
        backgroundUrl: '../img/icons.png'
    });
});

var iconTasks = ['icons:1x', 'icons:2x', 'icons:1x:white', 'icons:2x:white'];
gulp.task('icons', iconTasks);

gulp.task('less:viewer', ['icons'], function() {
    return ns.pipeline.obj(
        gulp.src('./less/viewer.less'),
        $.sourcemaps.init(),
        $.less(),
        $.sourcemaps.write({
            destPath: './css/viewer.css.map',
            sourceMappingURL: function() {
                return 'viewer.css.map';
            }
        }),
        gulp.dest('./css')
    );
});

gulp.task('less:fonts', ['icons'], function() {
    return ns.pipeline.obj(
        gulp.src('./less/fonts.less'),
        $.sourcemaps.init(),
        $.less(),
        $.sourcemaps.write({
            destPath: './css/fonts.css.map',
            sourceMappingURL: function() {
                return 'fonts.css.map';
            }
        }),
        gulp.dest('./css')
    );
});

gulp.task('less:legacy', ['icons'], function() {
    return ns.pipeline.obj(
        gulp.src('./less/legacy.less'),
        $.sourcemaps.init(),
        $.less(),
        $.sourcemaps.write({
            destPath: './css/legacy.css.map',
            sourceMappingURL: function() {
                return 'legacy.css.map';
            }
        }),
        gulp.dest('./css')
    );
});

gulp.task('less:dev', ['less:viewer', 'less:fonts', 'less:legacy']);

gulp.task('less:prod', ['icons'], function() {
    return ns.pipeline.obj(
        gulp.src(['./less/viewer.less', './less/fonts.less', './less/legacy.less']),
        $.less(),
        gulp.dest('./css')
    );
});

gulp.task('builddev', ['less:dev', 'icons']);
gulp.task('buildprod', ['less:prod', 'icons']);

gulp.task('watch', function(callback) {
    gulp.watch(['./less/**/*.less', './icons/**/*.svg'], ['builddev']);
});