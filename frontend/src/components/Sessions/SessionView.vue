<template>
  <b-container>
    <h1 v-t="'user.session.viewTitle'" />
    <status-detail :createdOn="new Date(session.createdOn)" :updatedOn="session.updatedOn ? new Date(session.updatedOn) : null" />
    <table class="table table-striped">
      <tbody>
        <tr>
          <th scope="row" v-t="'user.session.user'" />
          <td>
            <b-link :href="`/users/${session.user.id}`" class="mx-1"><user-avatar :user="session.user" /></b-link>
            <b-link :href="`/users/${session.user.id}`">{{ session.user.fullName || session.user.username }}</b-link>
          </td>
        </tr>
        <tr>
          <th scope="row" v-t="'user.session.persistent.label'" />
          <td v-text="$t(session.isPersistent ? 'yes' : 'no')" />
        </tr>
        <tr>
          <th scope="row" v-t="'user.session.signedOutOn'" />
          <td>
            <status-cell v-if="session.signedOutOn" :actor="session.signedOutBy" :date="new Date(session.signedOutOn)" />
            <b-badge v-else-if="session.isActive" variant="info">{{ $t('user.session.active.label') }}</b-badge>
          </td>
        </tr>
        <tr v-if="session.ipAddress">
          <th scope="row" v-t="'user.session.ipAddress'" />
          <td v-text="session.ipAddress" />
        </tr>
      </tbody>
    </table>
    <h3 v-t="'user.session.additionalInformation.title'" />
    <json-viewer v-if="session.additionalInformation" :value="JSON.parse(session.additionalInformation)" :expand-depth="5" copyable boxed />
    <p v-else v-t="'user.session.additionalInformation.empty'" />
  </b-container>
</template>

<script>
import UserAvatar from '@/components/User/UserAvatar.vue'

export default {
  name: 'SessionView',
  components: {
    UserAvatar
  },
  props: {
    json: {
      type: String,
      required: true
    }
  },
  data() {
    return {
      session: null
    }
  },
  created() {
    this.session = JSON.parse(this.json)
  }
}
</script>
