"use strict";

var gulp = require('gulp'),
    sass = require('gulp-sass');

var paths = {
    webroot: "./wwwroot/"
};

gulp.task("style", function () {
    return gulp.src(paths.webroot + 'scss/style.scss')
        .pipe(sass())
        .pipe(gulp.dest(paths.webroot + '/css'));
});

gulp.task("icons", function () {
    return gulp.src(paths.webroot + 'scss/icons.scss')
        .pipe(sass())
        .pipe(gulp.dest(paths.webroot + '/css'));
});