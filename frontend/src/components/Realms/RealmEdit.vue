<template>
  <b-container>
    <h1 v-t="realm ? 'realms.editTitle' : 'realms.newTitle'" />
    <status-detail v-if="realm" :createdAt="new Date(realm.createdAt)" :updatedAt="realm.updatedAt ? new Date(realm.updatedAt) : null" />
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <div class="my-2">
          <icon-submit v-if="realm" :disabled="!hasChanges || loading" icon="save" :loading="loading" text="actions.save" variant="primary" />
          <icon-submit v-else :disabled="!hasChanges || loading" icon="plus" :loading="loading" text="actions.create" variant="success" />
        </div>
        <alias-field v-if="realm" disabled :value="alias" />
        <alias-field v-else :name="name" required validate v-model="alias" />
        <name-field required v-model="name" />
        <description-field :rows="15" v-model="description" />
      </b-form>
    </validation-observer>
  </b-container>
</template>

<script>
import AliasField from './AliasField.vue'
import { createRealm, updateRealm } from '@/api/realms'

export default {
  name: 'RealmEdit',
  components: {
    AliasField
  },
  props: {
    json: {
      type: String,
      default: ''
    },
    status: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      alias: null,
      description: null,
      loading: false,
      name: null,
      realm: null
    }
  },
  computed: {
    hasChanges() {
      return (
        (this.alias ?? '') !== (this.realm?.alias ?? '') ||
        (this.name ?? '') !== (this.realm?.name ?? '') ||
        (this.description ?? '') !== (this.realm?.description ?? '')
      )
    },
    payload() {
      const payload = {
        name: this.name,
        description: this.description
      }
      if (!this.realm) {
        payload.alias = this.alias
      }
      return payload
    }
  },
  methods: {
    setModel(realm) {
      this.realm = realm
      this.alias = realm.alias
      this.description = realm.description
      this.name = realm.name
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            if (this.realm) {
              const { data } = await updateRealm(this.realm.id, this.payload)
              this.setModel(data)
              this.toast('success', 'realms.updated')
              this.$refs.form.reset()
            } else {
              const { data } = await createRealm(this.payload)
              window.location.replace(`/realms/${data.id}?status=created`)
            }
          }
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  },
  created() {
    if (this.json) {
      this.setModel(JSON.parse(this.json))
    }
    if (this.status === 'created') {
      this.toast('success', 'realms.created')
    }
  }
}
</script>
