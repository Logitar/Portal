module.exports = {
  filenameHashing: false,
  outputDir: '../Logitar.Portal.v2.Web/wwwroot/dist',
  publicPath: '/dist',
  runtimeCompiler: true,
  productionSourceMap: false,
  css: {
    extract: {
      filename: '[name].css',
      chunkFilename: '[name].css'
    }
  },
  configureWebpack: {
    output: {
      filename: '[name].js',
      chunkFilename: '[name].js'
    }
  },
  chainWebpack: config => {
    config.plugins.delete('html')
    config.plugins.delete('preload')
    config.plugins.delete('prefetch')
    config.module
      .rule('i18n')
      .resourceQuery(/blockType=i18n/)
      .type('javascript/auto')
      .use('i18n')
      .loader('@kazupon/vue-i18n-loader')
      .end()
  }
}
