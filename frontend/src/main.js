import Vue from 'vue'
import Gravatar from 'vue-gravatar'
import components from './components'
import i18n from './i18n'
import './config/bootstrap'
import './config/fontAwesome'
import './config/vee-validate'
import './global'

Vue.config.productionTip = false

Vue.component('v-gravatar', Gravatar)

new Vue({
  components,
  i18n
}).$mount('#app')
