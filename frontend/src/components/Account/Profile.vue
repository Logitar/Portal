<template>
  <b-container>
    <h1 v-t="'user.profile.title'" />
    <personal-information :profile="profile" />
    <authentication-information :profile="profile" @updated="setModel" />
  </b-container>
</template>

<script>
import AuthenticationInformation from './Profile/AuthenticationInformation.vue'
import PersonalInformation from './Profile/PersonalInformation.vue'

export default {
  name: 'Profile',
  components: {
    AuthenticationInformation,
    PersonalInformation
  },
  props: {
    json: {
      type: String,
      required: true
    }
  },
  data() {
    return {
      profile: null
    }
  },
  methods: {
    setModel(profile) {
      this.profile = profile
    }
  },
  created() {
    this.setModel(JSON.parse(this.json))
  }
}
</script>
