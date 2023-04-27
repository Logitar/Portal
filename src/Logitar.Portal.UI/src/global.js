import Vue from 'vue'

import CountSelect from './components/shared/CountSelect.vue'
import DeleteModal from './components/shared/DeleteModal.vue'
import DescriptionField from './components/shared/DescriptionField.vue'
import FormDateTime from './components/shared/FormDateTime.vue'
import FormField from './components/shared/FormField.vue'
import FormSelect from './components/shared/FormSelect.vue'
import FormTextarea from './components/shared/FormTextarea.vue'
import FormUrl from './components/shared/FormUrl.vue'
import IconButton from './components/shared/IconButton.vue'
import IconSubmit from './components/shared/IconSubmit.vue'
import JwtSecretField from './components/shared/JwtSecretField.vue'
import LocaleSelect from './components/shared/LocaleSelect.vue'
import NameField from './components/shared/NameField.vue'
import PasswordSettings from './components/shared/PasswordSettings.vue'
import SearchField from './components/shared/SearchField.vue'
import SortSelect from './components/shared/SortSelect.vue'
import StatusCell from './components/shared/StatusCell.vue'
import StatusDetail from './components/shared/StatusDetail.vue'
import StatusInfo from './components/shared/StatusInfo.vue'
import TimeZoneSelect from './components/shared/TimeZoneSelect.vue'
import UsernameSettings from './components/shared/UsernameSettings.vue'

Vue.component('count-select', CountSelect)
Vue.component('delete-modal', DeleteModal)
Vue.component('description-field', DescriptionField)
Vue.component('form-datetime', FormDateTime)
Vue.component('form-field', FormField)
Vue.component('form-select', FormSelect)
Vue.component('form-textarea', FormTextarea)
Vue.component('form-url', FormUrl)
Vue.component('icon-button', IconButton)
Vue.component('icon-submit', IconSubmit)
Vue.component('jwt-secret-field', JwtSecretField)
Vue.component('locale-select', LocaleSelect)
Vue.component('name-field', NameField)
Vue.component('password-settings', PasswordSettings)
Vue.component('search-field', SearchField)
Vue.component('sort-select', SortSelect)
Vue.component('status-cell', StatusCell)
Vue.component('status-detail', StatusDetail)
Vue.component('status-info', StatusInfo)
Vue.component('timezone-select', TimeZoneSelect)
Vue.component('username-settings', UsernameSettings)

Vue.mixin({
  methods: {
    getValidationState({ dirty, validated, valid = null }) {
      return dirty || validated ? valid : null
    },
    handleError(e = null) {
      if (e) {
        console.error(e)
      }
      this.toast('errorToast.title', 'errorToast.body', 'danger')
    },
    orderBy(items, key = null) {
      return typeof key !== 'undefined' && key !== null
        ? [...items].sort((a, b) => (a[key] < b[key] ? -1 : a[key] > b[key] ? 1 : 0))
        : [...items].sort((a, b) => (a < b ? -1 : a > b ? 1 : 0))
    },
    toast(title, body = '', variant = 'success') {
      this.$bvToast.toast(this.$i18n.t(body), {
        solid: true,
        title: this.$i18n.t(title),
        variant
      })
    }
  }
})
